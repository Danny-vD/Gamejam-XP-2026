using System;
using System.Collections.Generic;
using System.Linq;
using VDFramework.Extensions;
using VDPackages.SavePackage.Parsing;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;

namespace VDPackages.SavePackage.FileManagement
{
	public static partial class FileManager // Contains most functions again but then with a generic type parameter for convenience
	{
		/// <summary>
		/// <para>Create a new file with the given metadata and returns the index in the internal <see cref="AbstractSavableFile"/> array</para>
		/// <para>The new file is not immediately saved to disk, you will have to call <see cref="SaveFileAtIndex"/> for that.</para>
		/// <para>To immediately start using the file through <see cref="GetCurrentFile{TSaveFileType}"/> you can use <see cref="SwitchCurrentFileToFileAtIndex"/> with the returned index</para>
		/// </summary>
		/// <param name="metaData">The meta data of this file, this will be used to determine where to save it</param>
		/// <param name="saveFile">The new file</param>
		/// <returns>The index of the new file in the internal loaded files array</returns>
		public static int CreateNewFileOfType<TSaveFileType>(MetaData metaData, out TSaveFileType saveFile) where TSaveFileType : AbstractSavableFile, new()
		{
			saveFile = new TSaveFileType();
			saveFile.SetMetaData(metaData);
			saveFile.SetAsNewFile();

			Type fileType = typeof(TSaveFileType);
			List<AbstractSavableFile> loadedFiles = loadedFilesPerType.GetOrAddNewValue(fileType);
			int index = loadedFiles.Count;

			loadedFiles.Add(saveFile);

			OnNewFileCreated.Invoke(fileType, saveFile);

			return index;
		}
		
		/// <summary>
		/// <para>Returns a <see cref="MetaData"/> array which represents all the save files on disk</para>
		/// </summary>
		/// <remarks>This array is not necessarily in the same order as <see cref="GetAllSaveFiles{TFileType}"/>, which only represents the files that were actually parsed into an object</remarks>
		/// <returns>An array representing all the files on disk of the given type</returns>
		/// <seealso cref="GetAllAvailableSavesFromSubfolder{TFileType}"/>
		public static MetaData[] GetAllFilesOfTypeOnDisk<TFileType>() where TFileType : AbstractSavableFile
		{
			return FileParser.GetAllFilesOfType(typeof(TFileType));
		}

		/// <summary>
		/// <para>Returns a <see cref="MetaData"/> array which represents all the save files on disk within a given subfolder</para>
		/// </summary>
		/// <param name="subFolder">A path that represents any additional folders within the save directory, should not end in a DirectorySeperatorChar</param>
		/// <param name="includeNestedFolders">If true, the metadata from files in deeper folders will also be included</param>
		/// <returns>An array representing all the savefiles on disk within the given folder structure</returns>
		/// <seealso cref="GetAllFilesOfTypeOnDisk{TFileType}"/>
		public static MetaData[] GetAllFilesOfTypeOnDiskWithinSubfolder<TFileType>(string subFolder, bool includeNestedFolders = true) where TFileType : AbstractSavableFile
		{
			subFolder = subFolder.Trim(PathConstants.DirectorySeperatorCharacters);
			
			return FileParser.GetAllFilesOfTypeInSubfolder(typeof(TFileType), subFolder, includeNestedFolders);
		}

		/// <summary>
		/// Load the save file with the given metadata into memory by parsing it into a <see cref="AbstractSavableFile"/> object
		/// </summary>
		public static TSaveFileType LoadFile<TSaveFileType>(MetaData fileLocation, out int index) where TSaveFileType : AbstractSavableFile
		{
			return (TSaveFileType)LoadFile(typeof(TSaveFileType), fileLocation, out index);
		}

		/// <summary>
		/// Get all available saves from disk and parse them into objects
		/// </summary>
		/// <remarks>This sets the internal <see cref="AbstractSavableFile"/> array and therefore overrides any unsaved changes</remarks>
		public static void LoadAllFilesOfType<TSaveFileType>() where TSaveFileType : AbstractSavableFile
		{
			LoadAllFilesOfType(typeof(TSaveFileType));
		}

		/// <summary>
		/// Save the save file at the given index to disk
		/// </summary>
		public static void SaveFileAtIndex<TFileType>(int index) where TFileType : AbstractSavableFile
		{
			Type fileType = typeof(TFileType);
			SaveFileAtIndex(fileType, index);
		}

		/// <summary>
		/// Save the current save file to disk if one exists
		/// </summary>
		public static void SaveCurrentFileOfType<TFileType>() where TFileType : AbstractSavableFile
		{
			SaveCurrentFileOfType(typeof(TFileType));
		}

		/// <summary>
		/// <para>Returns a shallow copy of the internally stored <see cref="AbstractSavableFile"/> array.
		/// Which contains all savefiles of the given type that are loaded into memory</para>
		/// <para>These are the savefiles who have been parsed into an object and are available to get information from</para>
		/// </summary>
		/// <returns>An array representing all the savefiles that were parsed</returns>
		public static AbstractSavableFile[] GetAllLoadedFilesOfType<TFileType>() where TFileType : AbstractSavableFile
		{
			return GetAllLoadedFilesOfType(typeof(TFileType));
		}

		/// <summary>
		/// <para>Returns the file stored at <paramref name="index"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <param name="index">The index of the file to return</param>
		/// <typeparam name="TFileType">The type of the file to return</typeparam>
		public static TFileType GetFileAtIndex<TFileType>(int index) where TFileType : AbstractSavableFile
		{
			if (index < 0)
			{
				return null;
			}

			if (!loadedFilesPerType.TryGetValue(typeof(TFileType), out List<AbstractSavableFile> files))
			{
				return null;
			}

			return index < files.Count ? (TFileType)files[index] : null;
		}

		/// <summary>
		/// The amount of filesof the given type currently loaded into memory (parsed into an object)
		/// </summary>
		/// <remarks>The returned value is equal to the length of the array of <see cref="GetAllLoadedFilesOfType{TFileType}"/> (but without making any copies)</remarks>
		public static int GetFileCountForType<TFileType>()
		{
			return GetFileCountForType(typeof(TFileType));
		}

		/// <summary>
		/// Get the index of the first save file that matches the <paramref name="predicate"/>
		/// </summary>
		/// <param name="predicate">A function to test each file for a condition</param>
		/// <returns>The index of the first file that met the conditions, or -1 if none did</returns>
		public static int GetIndexOfFirstMatchingFile<TFileType>(Func<TFileType, bool> predicate, out TFileType matchingFile) where TFileType : AbstractSavableFile
		{
			IEnumerable<TFileType> files = loadedFilesPerType.GetOrAddNewValue(typeof(TFileType)).Cast<TFileType>();

			int index = 0;

			foreach (TFileType file in files)
			{
				if (predicate.Invoke(file))
				{
					matchingFile = file;
					return index;
				}

				++index;
			}

			matchingFile = null;
			return -1;
		}

		/// <summary>
		/// Try to get the index of the first save file that matches the <paramref name="predicate"/>
		/// </summary>
		/// <param name="predicate">A function to test each file for a condition</param>
		/// <returns>An array that contains all the indices of the files that met the conditions</returns>
		public static List<int> GetIndicesOfAllMatchingFiles<TFileType>(Func<TFileType, bool> predicate) where TFileType : AbstractSavableFile
		{
			List<int> indices = new List<int>();

			IEnumerable<TFileType> files = loadedFilesPerType.GetOrAddNewValue(typeof(TFileType)).Cast<TFileType>();

			int index = 0;

			foreach (TFileType file in files)
			{
				if (predicate.Invoke(file))
				{
					indices.Add(index);
				}

				++index;
			}

			return indices;
		}

		/// <summary>
		/// The index of the current save file or -1 if there is none available
		/// </summary>
		public static int GetIndexOfCurrentFile<TFileType>()
		{
			return currentFileIndexPerType.GetOrAddNewValue(typeof(TFileType), -1);
		}

		/// <summary>
		/// <para>Returns the file corresponding to the <see cref="GetIndexOfCurrentFile{TFileType}"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <typeparam name="TFileType">The type of the file to return</typeparam>
		public static TFileType GetCurrentFile<TFileType>() where TFileType : AbstractSavableFile
		{
			return (TFileType)GetCurrentFile(typeof(TFileType));
		}

		/// <summary>
		/// <para>Returns the file corresponding to the <see cref="GetIndexOfCurrentFile{TFileType}"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <typeparam name="TFileType">The type of the file to return</typeparam>
		public static TFileType GetCurrentFile<TFileType>(out int currentIndex) where TFileType : AbstractSavableFile
		{
			return (TFileType)GetCurrentFile(typeof(TFileType), out currentIndex);
		}

		/// <summary>
		/// True if <see cref="GetCurrentFile{TFileType}"/> will return a valid file
		/// </summary>
		public static bool HasCurrentFile<TFileType>()
		{
			return HasCurrentFile(typeof(TFileType));
		}

		/// <summary>
		/// <para>True if the current file has unsaved changes</para>
		/// <para>False if there is no current file or it has no unsaved changes</para>
		/// </summary>
		public static bool IsCurrentFileDirty<TFileType>() where TFileType : AbstractSavableFile
		{
			return IsCurrentFileDirty(typeof(TFileType));
		}

		/// <summary>
		/// Change which savefile will be returned with <see cref="GetCurrentFile{TSaveFileType}"/>
		/// </summary>
		/// <param name="index">The index of the savefile to switch to</param>
		/// <param name="autosaveOnSwitch">If true, the current savefile (if valid) will be saved if it is dirty</param>
		/// <callbacks>Triggers <see cref="OnCurrentFileChanged"/> on a successful change</callbacks>
		public static void SetCurrentFileToIndex<TFileType>(int index, bool autosaveOnSwitch) where TFileType : AbstractSavableFile
		{
			SetCurrentFileToIndex(typeof(TFileType), index, autosaveOnSwitch);
		}

		public static int DuplicateFileAtIndex<TFileType>(int index, MetaData metaDataOfDuplicate, out TFileType duplicate) where TFileType : AbstractSavableFile
		{
			int returnedIndex = DuplicateFileAtIndex(typeof(TFileType), index, metaDataOfDuplicate, out AbstractSavableFile duplicateFile);
			duplicate = (TFileType)duplicateFile;

			return returnedIndex;
		}

		public static int DuplicateCurrentFile<TFileType>(MetaData metaDataOfDuplicate, out TFileType duplicate) where TFileType : AbstractSavableFile
		{
			int returnedIndex = DuplicateCurrentFile(typeof(TFileType), metaDataOfDuplicate, out AbstractSavableFile duplicateFile);
			duplicate = (TFileType)duplicateFile;

			return returnedIndex;
		}

		/// <summary>
		/// <para>Restores the savefile at the given index to the state it was in at the last time it was saved</para>
		/// <para>This deletes all unsaved changes and competely resets the save in case the savefile has not been saved at all</para>
		/// </summary>
		/// <param name="index">The index of the savefile to reload</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="KeyNotFoundException">There was never a file of the given type loaded into memory</exception>
		public static void ReloadFileAtIndex<TFileType>(int index) where TFileType : AbstractSavableFile
		{
			ReloadFileAtIndex(typeof(TFileType), index);
		}

		/// <summary>
		/// <para>Restores the current file to the state it was in at the last time it was saved</para>
		/// <para>This deletes all unsaved changes and competely resets the save in case the file has not been saved at all</para>
		/// </summary>
		public static void ReloadCurrentFile<TFileType>() where TFileType : AbstractSavableFile
		{
			ReloadCurrentFile(typeof(TFileType));
		}

		/// <summary>
		/// Resets the save file at the given index
		/// </summary>
		public static void ResetFileAtIndex<TFileType>(int index)
		{
			ResetFileAtIndex(typeof(TFileType), index);
		}

		/// <summary>
		/// Resets the current file if one exists 
		/// </summary>
		public static void ResetCurrentFile<TFileType>() where TFileType : AbstractSavableFile
		{
			ResetCurrentFile(typeof(TFileType));
		}

		/// <summary>
		/// Delete the file with the given meta data. Nothing happens if the file does not exist
		/// </summary>
		/// <param name="metaData">The meta data of the file to delete</param>
		public static void DeleteFile<TFileType>(MetaData metaData) where TFileType : AbstractSavableFile
		{
			DeleteFile(typeof(TFileType), metaData);
		}
		
		/// <summary>
		/// Delete the current file if one exists
		/// </summary>
		public static void DeleteCurrentFile<TFileType>() where TFileType : AbstractSavableFile
		{
			DeleteCurrentFile(typeof(TFileType));
		}

		/// <summary>
		/// <para>Deletes the directory of saves at the given path</para>
		/// <para>This effectively deletes all the savefiles at the given directory and below</para>
		/// <para>If the <paramref name="directoryPath"/> is an empty string it will delete <b>all</b> files of the given type</para>
		/// </summary>
		/// <param name="directoryPath">the subpath to the save directory to delete (start/end directorySeperator chars will be trimmed)</param>
		/// <seealso cref="DeleteAllFilesOfType{TFileType}"/>
		public static void DeleteFileDirectory<TFileType>(string directoryPath) where TFileType : AbstractSavableFile
		{
			DeleteFileDirectory(typeof(TFileType), directoryPath);
		}

		/// <summary>
		/// Delete all files of the given type
		/// </summary>
		public static void DeleteAllFilesOfType<TFileType>()
		{
			DeleteAllFilesOfType(typeof(TFileType));
		}
	}
}