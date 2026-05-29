using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VDFramework.DynamicInstantiation;
using VDFramework.EventSystem;
using VDFramework.Extensions;
using VDFramework.Logger;
using VDPackages.SavePackage.Events;
using VDPackages.SavePackage.Parsing;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.FileManagement
{
	/// <summary>
	/// An abstraction layer to easily manage different types of files (saving, loading, deleting etc.)
	/// </summary>
	public static partial class FileManager
	{
		public static event Action<Type, AbstractSavableFile> OnNewFileCreated = delegate { };
		public static event Action<Type, AbstractSavableFile> OnFileSaved = delegate { };
		public static event Action<Type> OnAllFilesOfTypeSaved = delegate { };
		public static event Action<Type, AbstractSavableFile> OnFileLoaded = delegate { };
		public static event Action<Type> OnAllFilesOfTypeLoaded = delegate { };
		public static event Action<Type, AbstractSavableFile, AbstractSavableFile> OnCurrentFileChanged = delegate { };
		public static event Action<Type> OnCurrentFileCleared = delegate { };
		public static event Action<Type, MetaData> OnFileDeleted = delegate { };
		public static event Action<Type> OnAllFilesOfTypeDeleted = delegate { };
		public static event Action<Type, string> OnFileDirectoryDeleted = delegate { };
		public static event Action OnDeletedEverything = delegate { };

		private static readonly Type[] constructorParameters = new Type[] { typeof(MetaData) };

		/// <summary>
		/// Some filestypes might have a concept of 'current' (like savefiles) this dictionary stores the index of the 'current' per type
		/// </summary>
		private static readonly Dictionary<Type, int> currentFileIndexPerType = new Dictionary<Type, int>();

		/// <summary>
		/// Represents the files on disk whose contents were parsed into an object
		/// </summary>
		private static readonly Dictionary<Type, List<AbstractSavableFile>> loadedFilesPerType = new Dictionary<Type, List<AbstractSavableFile>>();


		/// <summary>
		/// Reset the fields manually in case domain reload is disabled
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetInternalState()
		{
			OnNewFileCreated        = delegate { };
			OnFileSaved             = delegate { };
			OnAllFilesOfTypeSaved   = delegate { };
			OnFileLoaded            = delegate { };
			OnAllFilesOfTypeLoaded  = delegate { };
			OnCurrentFileChanged    = delegate { };
			OnCurrentFileCleared    = delegate { };
			OnFileDeleted           = delegate { };
			OnAllFilesOfTypeDeleted = delegate { };
			OnFileDirectoryDeleted  = delegate { };
			OnDeletedEverything     = delegate { };

			loadedFilesPerType?.Clear();
			currentFileIndexPerType?.Clear();
		}

		public static int CreateNewFileOfType(Type fileType, MetaData metaData, out AbstractSavableFile file)
		{
			Func<MetaData, AbstractSavableFile> constructor = ConstructorCreator.GetConstructor<Func<MetaData, AbstractSavableFile>>(fileType, constructorParameters);
			file = constructor.Invoke(metaData);

			List<AbstractSavableFile> loadedFiles = loadedFilesPerType.GetOrAddNewValue(fileType);
			int index = loadedFiles.Count;

			loadedFiles.Add(file);

			OnNewFileCreated.Invoke(fileType, file);

			return index;
		}

		/// <summary>
		/// <para>Returns a <see cref="MetaData"/> array which represents all the save files on disk</para>
		/// </summary>
		/// <remarks>This array is not necessarily in the same order as <see cref="GetAllSaveFiles{TFileType}"/>, which only represents the files that were actually parsed into an object</remarks>
		/// <returns>An array representing all the files on disk of the given type</returns>
		/// <seealso cref="GetAllAvailableSavesFromSubfolder{TFileType}"/>
		public static MetaData[] GetAllFilesOfTypeOnDisk(Type fileType)
		{
			return FileParser.GetAllFilesOfType(fileType);
		}

		/// <summary>
		/// <para>Returns a <see cref="MetaData"/> array which represents all the save files on disk within a given subfolder</para>
		/// </summary>
		/// <param name="subFolder">A path that represents any additional folders within the save directory, should not end in a DirectorySeperatorChar</param>
		/// <param name="includeNestedFolders">If true, the metadata from files in deeper folders will also be included</param>
		/// <returns>An array representing all the savefiles on disk within the given folder structure</returns>
		/// <seealso cref="GetAllFilesOfTypeOnDisk{TFileType}"/>
		public static MetaData[] GetAllFilesOfTypeOnDiskWithinSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true)
		{
			subFolder = subFolder.Trim(PathConstants.DirectorySeperatorCharacters);

			return FileParser.GetAllFilesOfTypeInSubfolder(fileType, subFolder, includeNestedFolders);
		}

		/// <summary>
		/// Load the file with the given type and metadata into memory by parsing it into a <see cref="AbstractSavableFile"/> object
		/// </summary>
		/// <remarks>This will override unsaved changes if the given file was already loaded into memory and contains unsaved changes</remarks>
		public static AbstractSavableFile LoadFile(Type fileType, MetaData fileLocation, out int index)
		{
			AbstractSavableFile file = FileParser.Parse(fileType, fileLocation);

			if (file == null)
			{
				index = -1;
				return null;
			}

			if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> files)) // If there are no loaded files for the given type
			{
				loadedFilesPerType[fileType] = new List<AbstractSavableFile>() { file };
				index                        = 0;
			}
			else
			{
				index = files.IndexOf(file);

				if (index == -1) // If there are loaded files for the given type, but the just loaded file is not among them
				{
					index = files.Count;
					files.Add(file);
				}
				else // If there are loaded files for the given type and the loaded file is already among them
				{
					// Override the existing file (because we should not have the same file loaded into different objects)
					loadedFilesPerType[fileType][index] = file;
				}
			}

			OnFileLoaded.Invoke(fileType, file);

			return file;
		}

		/// <summary>
		/// Loads all files of all types from disk
		/// </summary>
		/// <remarks>This overrides the internal loaded files array and therefore overrides any unsaved changes if any file of the given type was already loaded into memory</remarks>
		public static void LoadAllFiles()
		{
			Type[] typesWithCurrentIndex = currentFileIndexPerType.Keys.ToArray();

			IEnumerable<(Type, AbstractSavableFile)> oldCurrentFiles = typesWithCurrentIndex.Select(fileType => (fileType, GetCurrentFile(fileType)));

			loadedFilesPerType.Clear();
			currentFileIndexPerType.Clear();

			AbstractSavableFile[] files = FileParser.ParseAllFiles();

			if (files.Length == 0)
			{
				return;
			}

			foreach (AbstractSavableFile file in files)
			{
				Type fileType = file.GetType();

				if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> loadedFiles))
				{
					loadedFiles = new List<AbstractSavableFile>();
					loadedFilesPerType.Add(fileType, loadedFiles);
				}

				loadedFiles.Add(file);
			}

			foreach (Type type in loadedFilesPerType.Keys)
			{
				OnAllFilesOfTypeLoaded.Invoke(type);
			}

			foreach ((Type fileType, AbstractSavableFile file) in oldCurrentFiles)
			{
				OnCurrentFileChanged.Invoke(fileType, file, null);
				OnCurrentFileCleared.Invoke(fileType);
			}
		}

		/// <summary>
		/// Get all available files of the given type from disk and load them into memory (parse into objects)
		/// </summary>
		/// <remarks>This overrides the internal loaded files array and therefore overrides any unsaved changes if any file of the given type was already loaded into memory</remarks>
		public static void LoadAllFilesOfType(Type fileType)
		{
			loadedFilesPerType[fileType] = FileParser.ParseAllFilesOfType(fileType).ToList();

			OnAllFilesOfTypeLoaded.Invoke(fileType);
		}

		/// <summary>
		/// Save the save file at the given index to disk
		/// </summary>
		/// <remarks>Will only save the file if it is marked as dirty</remarks>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="KeyNotFoundException">There was never a file of the given type loaded into memory</exception>
		public static void SaveFileAtIndex(Type fileType, int index)
		{
			AbstractSavableFile file = null;

			try
			{
				file = loadedFilesPerType[fileType][index];
			}
			catch (KeyNotFoundException)
			{
				LogManager.LogError($"Tried to get a file of type {fileType} while none are loaded!");
				throw;
			}
			catch (IndexOutOfRangeException)
			{
				LogManager.LogError($"Tried to get the file with index {index} while there are only {GetFileCountForType(fileType)} files for type: {fileType}");
				throw;
			}

			EventManager.RaiseEvent(new BeforeSaveEvent(fileType, file));

			if (!file.IsDirty && file.IsPresentOnDisk)
			{
				return;
			}

			FileParser.Save(file);

			OnFileSaved.Invoke(fileType, file);
		}

		/// <summary>
		/// Save the current save file to disk if one exists
		/// </summary>
		/// <remarks>Will only save the file if it is marked as dirty</remarks>
		public static void SaveCurrentFileOfType(Type fileType)
		{
			AbstractSavableFile file = GetCurrentFile(fileType);

			if (file == null)
			{
				return;
			}

			EventManager.RaiseEvent(new BeforeSaveEvent(fileType, file));

			if (file.IsDirty || !file.IsPresentOnDisk)
			{
				FileParser.Save(file);

				OnFileSaved.Invoke(fileType, file);
			}
		}

		/// <summary>
		/// Save the current file to disk for the types where it exists
		/// </summary>
		/// <remarks>Will only save the files marked as dirty and where the index leads to a valid file</remarks>
		public static void SaveAllCurrentFiles()
		{
			foreach (KeyValuePair<Type, int> pair in currentFileIndexPerType)
			{
				AbstractSavableFile currentFile = GetFileAtIndex(pair.Key, pair.Value);

				if (currentFile != null)
				{
					EventManager.RaiseEvent(new BeforeSaveEvent(pair.Key, currentFile));

					if (currentFile.IsDirty || !currentFile.IsPresentOnDisk)
					{
						FileParser.Save(currentFile);
						OnFileSaved.Invoke(pair.Key, currentFile);
					}
				}
			}
		}

		/// <summary>
		/// Save all loaded files to disk
		/// </summary>
		/// <remarks>Will only save the files marked as dirty</remarks>
		public static void SaveAllFiles()
		{
			foreach (KeyValuePair<Type, List<AbstractSavableFile>> pair in loadedFilesPerType)
			{
				bool anyFileSaved = false;

				foreach (AbstractSavableFile file in pair.Value)
				{
					EventManager.RaiseEvent(new BeforeSaveEvent(pair.Key, file));

					if (file.IsDirty || !file.IsPresentOnDisk)
					{
						FileParser.Save(file);

						anyFileSaved = true;
					}
				}

				if (anyFileSaved)
				{
					OnAllFilesOfTypeSaved.Invoke(pair.Key);
				}
			}
		}

		/// <summary>
		/// <para>Returns a shallow copy of the internally stored <see cref="AbstractSavableFile"/> array.
		/// Which contains all savefiles of the given type that are loaded into memory</para>
		/// <para>These are the savefiles who have been parsed into an object and are available to get information from</para>
		/// </summary>
		/// <returns>An array representing all the savefiles that were parsed</returns>
		public static AbstractSavableFile[] GetAllLoadedFilesOfType(Type fileType)
		{
			return loadedFilesPerType.GetOrAddNewValue(fileType).ToArray();
		}

		/// <summary>
		/// <para>Returns the file stored at <paramref name="index"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <param name="index">The index of the file to return</param>
		public static AbstractSavableFile GetFileAtIndex(Type fileType, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> files))
			{
				return null;
			}

			return index < files.Count ? files[index] : null;
		}

		/// <summary>
		/// The amount of files of the given type currently loaded into memory (parsed into an object)
		/// </summary>
		public static int GetFileCountForType(Type fileType)
		{
			if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> files))
			{
				return 0;
			}

			return files.Count;
		}

		/// <summary>
		/// <para>Get the index of the given file, or -1 if it does not exist</para>
		/// <para>Slightly faster than <see cref="GetIndexOfFile(AbstractSavableFile)"/> if you already have a reference to the type of the file</para>
		/// </summary>
		/// <seealso cref="ResetFileAtIndex"/>
		/// <seealso cref="SaveFileAtIndex"/>
		/// <seealso cref="SetCurrentFileToIndex"/>
		public static int GetIndexOfFile(Type fileType, AbstractSavableFile file)
		{
			List<AbstractSavableFile> files = loadedFilesPerType.GetOrAddNewValue(fileType);
			return files.IndexOf(file);
		}

		/// <summary>
		/// Get the index of the given file, or -1 if it does not exist
		/// </summary>
		/// <seealso cref="ResetFileAtIndex"/>
		/// <seealso cref="SaveFileAtIndex"/>
		/// <seealso cref="SetCurrentFileToIndex"/>
		public static int GetIndexOfFile(AbstractSavableFile file)
		{
			List<AbstractSavableFile> files = loadedFilesPerType.GetOrAddNewValue(file.GetType());
			return files.IndexOf(file);
		}

		/// <summary>
		/// Get the index of the first file that matches the <paramref name="predicate"/>
		/// </summary>
		/// <param name="predicate">A function to test each file for a condition</param>
		/// <returns>The index of the first file that met the conditions, or -1 if none did</returns>
		public static int GetIndexOfFirstMatchingFile(Type fileType, Func<AbstractSavableFile, bool> predicate, out AbstractSavableFile file)
		{
			List<AbstractSavableFile> files = loadedFilesPerType.GetOrAddNewValue(fileType);

			for (int i = 0; i < files.Count; i++)
			{
				file = files[i];

				if (predicate.Invoke(file))
				{
					return i;
				}
			}

			file = null;
			return -1;
		}

		/// <summary>
		/// Try to get the index of the first save file that matches the <paramref name="predicate"/>
		/// </summary>
		/// <param name="predicate">A function to test each file for a condition</param>
		/// <returns>An array that contains all the indices of the files that met the conditions</returns>
		public static List<int> GetIndicesOfAllMatchingFiles(Type fileType, Func<AbstractSavableFile, bool> predicate)
		{
			List<AbstractSavableFile> files = loadedFilesPerType.GetOrAddNewValue(fileType);
			List<int> indices = new List<int>(4);

			for (int i = 0; i < files.Count; i++)
			{
				AbstractSavableFile file = files[i];

				if (predicate.Invoke(file))
				{
					indices.Add(i);
				}
			}

			return indices;
		}

		/// <summary>
		/// The index of the current file for the given type or -1 if there is none available
		/// </summary>
		/// <remarks>If you want to get a reference to the current file, use <see cref="GetCurrentFile(System.Type)"/> directly</remarks>
		public static int GetIndexOfCurrentFile(Type fileType)
		{
			return currentFileIndexPerType.GetOrAddNewValue(fileType, -1);
		}

		/// <summary>
		/// <para>Returns the file corresponding to the <see cref="GetIndexOfCurrentFile{TFileType}"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <param name="fileType">The type of the file to return</param>
		public static AbstractSavableFile GetCurrentFile(Type fileType)
		{
			if (!currentFileIndexPerType.TryGetValue(fileType, out int currentIndex))
			{
				return null;
			}

			return GetFileAtIndex(fileType, currentIndex);
		}

		/// <summary>
		/// <para>Returns the file corresponding to the <see cref="GetIndexOfCurrentFile{TFileType}"/> in the internal file array</para>
		/// <para>Or NULL if the index is outside of the array</para>
		/// </summary>
		/// <param name="fileType">The type of the file to return</param>
		public static AbstractSavableFile GetCurrentFile(Type fileType, out int currentIndex)
		{
			if (!currentFileIndexPerType.TryGetValue(fileType, out currentIndex))
			{
				return null;
			}

			return GetFileAtIndex(fileType, currentIndex);
		}

		/// <summary>
		/// True if <see cref="GetCurrentFile"/> will return a valid file
		/// </summary>
		public static bool HasCurrentFile(Type fileType)
		{
			if (!currentFileIndexPerType.TryGetValue(fileType, out int currentIndex))
			{
				return false;
			}

			if (currentIndex < 0)
			{
				return false;
			}

			if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> files))
			{
				return false;
			}

			return currentIndex < files.Count;
		}

		/// <summary>
		/// <para>True if the current file has unsaved changes</para>
		/// <para>False if there is no current file or it has no unsaved changes</para>
		/// </summary>
		public static bool IsCurrentFileDirty(Type fileType)
		{
			return HasCurrentFile(fileType) && GetCurrentFile(fileType).IsDirty;
		}

		/// <summary>
		/// Set which file will be returned with <see cref="GetCurrentFile(System.Type)"/>
		/// </summary>
		/// <param name="currentIndex">The index of the file to set as current</param>
		/// <param name="autosaveOnSwitch">If true, the current file (if it exists) will be saved if it is dirty</param>
		/// <callbacks>Triggers <see cref="OnCurrentFileChanged"/> on a successful change</callbacks>
		public static void SetCurrentFileToIndex(Type fileType, int currentIndex, bool autosaveOnSwitch)
		{
			int previousCurrentIndex = GetIndexOfCurrentFile(fileType);

			if (currentIndex == previousCurrentIndex) // Don't do anything if the current index is already the one we try to set it to
			{
				return;
			}

			AbstractSavableFile[] files = GetAllLoadedFilesOfType(fileType);

			if (currentIndex < 0 || currentIndex >= files.Length) // The given index is outside of the bounds of the array
			{
				throw new IndexOutOfRangeException($"Index {currentIndex} is not valid!\nThere are {files.Length} files.");
			}

			bool previousIndexValid = previousCurrentIndex >= 0 && previousCurrentIndex < files.Length;

			if (autosaveOnSwitch && previousIndexValid)
			{
				SaveFileAtIndex(fileType, previousCurrentIndex);
			}

			currentFileIndexPerType[fileType] = currentIndex;

			AbstractSavableFile previousCurrentFile = previousIndexValid ? files[previousCurrentIndex] : null;
			AbstractSavableFile currentFile = files[currentIndex];
			
			OnCurrentFileChanged.Invoke(fileType, previousCurrentFile, currentFile);
		}

		public static int DuplicateFileAtIndex(Type fileType, int index, MetaData metaDataOfDuplicate, out AbstractSavableFile duplicate)
		{
			List<AbstractSavableFile> files;
			AbstractSavableFile file;

			try
			{
				files = loadedFilesPerType[fileType];
				file  = files[index];
			}
			catch (KeyNotFoundException)
			{
				LogManager.LogError($"Tried to get a file of type {fileType} while none are loaded!");
				throw;
			}
			catch (IndexOutOfRangeException _)
			{
				LogManager.LogError($"Tried to get the file with index {index} while there are only {GetFileCountForType(fileType)} files for type: {fileType}");
				throw;
			}

			duplicate = file.Duplicate(metaDataOfDuplicate);

			int duplicateIndex = files.Count;
			files.Add(duplicate);

			return duplicateIndex;
		}

		public static int DuplicateCurrentFile(Type fileType, MetaData metaDataOfDuplicate, out AbstractSavableFile duplicate)
		{
			AbstractSavableFile currentFile = GetCurrentFile(fileType);

			if (currentFile != null)
			{
				duplicate = currentFile.Duplicate(metaDataOfDuplicate);
				List<AbstractSavableFile> files = loadedFilesPerType[fileType]; // Guaranteed to contain this key, otherwise current would be NULL

				int duplicateIndex = files.Count;
				files.Add(duplicate);

				return duplicateIndex;
			}

			duplicate = null;
			return -1;
		}

		/// <summary>
		/// <para>Restores the savefile at the given index to the state it was in at the last time it was saved</para>
		/// <para>This deletes all unsaved changes and competely resets the save in case the savefile has not been saved at all</para>
		/// </summary>
		/// <param name="index">The index of the savefile to reload</param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="KeyNotFoundException">There was never a file of the given type loaded into memory</exception>
		public static void ReloadFileAtIndex(Type fileType, int index)
		{
			AbstractSavableFile file;

			try
			{
				file = loadedFilesPerType[fileType][index];
			}
			catch (KeyNotFoundException)
			{
				LogManager.LogError($"Tried to get a file of type {fileType} while none are loaded!");
				throw;
			}
			catch (IndexOutOfRangeException _)
			{
				LogManager.LogError($"Tried to get the file with index {index} while there are only {GetFileCountForType(fileType)} files for type: {fileType}");
				throw;
			}

			// No valid metadata (the file is not linked to any valid position on disk)
			if (!file.MetaData.IsValid())
			{
				file.Reset();
				return;
			}

			AbstractSavableFile newFile = FileParser.Parse(fileType, file.MetaData);

			// Parsing failed, so is not present on disk (might never have been saved yet). In this case, we can just reset the file
			if (newFile == null)
			{
				file.Reset();
				return;
			}

			loadedFilesPerType[fileType][index] = newFile;
		}

		/// <summary>
		/// <para>Restores the current file to the state it was in at the last time it was saved</para>
		/// <para>This deletes all unsaved changes and competely resets the save in case the file has not been saved at all</para>
		/// </summary>
		/// <seealso cref="ReloadFileAtIndex"/>
		public static void ReloadCurrentFile(Type fileType)
		{
			int currentIndex = GetIndexOfCurrentFile(fileType);

			if (currentIndex == -1)
			{
				return;
			}

			if (!loadedFilesPerType.TryGetValue(fileType, out List<AbstractSavableFile> files))
			{
				return;
			}

			if (currentIndex >= files.Count)
			{
				return;
			}

			AbstractSavableFile currentFile = loadedFilesPerType[fileType][currentIndex];

			ReloadFileAtIndex(fileType, currentIndex);
			OnCurrentFileChanged.Invoke(fileType, currentFile, currentFile);
		}

		/// <summary>
		/// Resets the save file at the given index
		/// </summary>
		public static void ResetFileAtIndex(Type fileType, int index)
		{
			AbstractSavableFile file;

			try
			{
				file = loadedFilesPerType[fileType][index];
			}
			catch (KeyNotFoundException)
			{
				LogManager.LogError($"Tried to get a file of type {fileType} while none are loaded!");
				throw;
			}
			catch (IndexOutOfRangeException _)
			{
				LogManager.LogError($"Tried to get the file with index {index} while there are only {GetFileCountForType(fileType)} files for type: {fileType}");
				throw;
			}

			file.Reset();
		}

		/// <summary>
		/// Resets the current file if one exists 
		/// </summary>
		public static void ResetCurrentFile(Type fileType)
		{
			AbstractSavableFile currentFile = GetCurrentFile(fileType);

			if (currentFile != null)
			{
				currentFile.Reset();
				OnCurrentFileChanged.Invoke(fileType, currentFile, currentFile); // Added for convenience; a reset is almost the same as changing to another file
			}
		}

		/// <summary>
		/// Resets all current files
		/// </summary>
		public static void ResetAllCurrentFiles()
		{
			foreach (KeyValuePair<Type, int> pair in currentFileIndexPerType)
			{
				AbstractSavableFile currentFile = GetFileAtIndex(pair.Key, pair.Value);

				if (currentFile != null)
				{
					currentFile.Reset();
					OnCurrentFileChanged.Invoke(pair.Key, currentFile, currentFile); // Added for convenience; a reset is almost the same as changing to another file
				}
			}
		}

		/// <summary>
		/// Delete the file with the given meta data. Nothing happens if the file does not exist
		/// </summary>
		/// <param name="metaData">The meta data of the file to delete</param>
		public static void DeleteFile(Type fileType, MetaData metaData)
		{
			FileParser.DeleteFile(fileType, metaData);

			int indexToRemove = GetIndexOfFirstMatchingFile(fileType, file => file.MetaData.Equals(metaData), out AbstractSavableFile fileToRemove);

			if (indexToRemove != -1)
			{
				// Remove the deleted file from the loaded files
				loadedFilesPerType[fileType].RemoveAt(indexToRemove);

				// Update the current file index if present and if the index of the file deleted is lower or equal to the current index
				if (currentFileIndexPerType.TryGetValue(fileType, out int currentIndex))
				{
					if (indexToRemove == currentIndex)
					{
						currentFileIndexPerType[fileType] = -1;
						OnCurrentFileChanged.Invoke(fileType, fileToRemove, null);
						OnCurrentFileCleared.Invoke(fileType);
					}
					else if (indexToRemove < currentIndex)
					{
						--currentFileIndexPerType[fileType];
					}
				}
			}

			OnFileDeleted.Invoke(fileType, metaData);
		}

		/// <summary>
		/// Delete the current file if one exists
		/// </summary>
		public static void DeleteCurrentFile(Type fileType)
		{
			AbstractSavableFile currentFile = GetCurrentFile(fileType, out int currentIndex);

			if (currentFile != null)
			{
				MetaData metaData = currentFile.MetaData;
				FileParser.DeleteFile(fileType, metaData);

				loadedFilesPerType[fileType].RemoveAt(currentIndex);
				currentFileIndexPerType[fileType] = -1;

				OnFileDeleted.Invoke(fileType, metaData);
				OnCurrentFileChanged.Invoke(fileType, currentFile, null);
				OnCurrentFileCleared.Invoke(fileType);
			}
		}

		/// <summary>
		/// <para>Deletes the directory of saves at the given path</para>
		/// <para>This effectively deletes all the savefiles at the given directory and below</para>
		/// <para>If the <paramref name="directoryPath"/> is an empty string it will delete <b>all</b> files</para>
		/// </summary>
		/// <param name="directoryPath">the subpath to the save directory to delete (start/end directorySeperator chars will be trimmed)</param>
		/// <seealso cref="DeleteAllData"/>
		public static void DeleteFileDirectory(Type fileType, string directoryPath)
		{
			if (string.IsNullOrEmpty(directoryPath))
			{
				DeleteAllFilesOfType(fileType);
				return;
			}

			directoryPath = directoryPath.Trim(PathConstants.DirectorySeperatorCharacters);

			FileParser.DeleteFileDirectory(fileType, directoryPath);

			string fullDirectoryPath = Path.Join(FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false), directoryPath);

			foreach (KeyValuePair<Type, List<AbstractSavableFile>> pair in loadedFilesPerType)
			{
				List<AbstractSavableFile> files = pair.Value;

				if (files.Count == 0)
				{
					continue;
				}

				string directoryPathForType = FileDirectoryHelper.GetRootDirectoryPathForType(pair.Key, false);

				if (!fullDirectoryPath.StartsWith(directoryPathForType))
				{
					// The directory to delete does not start with the root directory of this type, so no file of this type can match it.
					continue;
				}

				bool hasCurrentIndex = currentFileIndexPerType.TryGetValue(pair.Key, out int currentIndex);

				for (int fileIndex = files.Count - 1; fileIndex >= 0; fileIndex--)
				{
					AbstractSavableFile file = files[fileIndex];

					string pathOfFile = FileDirectoryHelper.GetDirectoryPathForFile(pair.Key, file.MetaData, false);

					if (pathOfFile.StartsWith(fullDirectoryPath))
					{
						files.RemoveAt(fileIndex);

						if (hasCurrentIndex && currentIndex != -1)
						{
							if (currentIndex == fileIndex)
							{
								// The current file was deleted so there's no current file anymore
								currentIndex = -1;
								OnCurrentFileChanged.Invoke(pair.Key, file, null);
								OnCurrentFileCleared.Invoke(pair.Key);
							}
							else if (fileIndex < currentIndex)
							{
								--currentIndex;
							}
						}
					}
				}

				if (hasCurrentIndex)
				{
					currentFileIndexPerType[fileType] = currentIndex;
				}
			}

			OnFileDirectoryDeleted.Invoke(fileType, directoryPath);
		}

		/// <summary>
		/// Delete all files of the given type
		/// </summary>
		public static void DeleteAllFilesOfType(Type fileType)
		{
			FileParser.DeleteAllFilesOfType(fileType);

			if (currentFileIndexPerType.ContainsKey(fileType))
			{
				currentFileIndexPerType[fileType] = -1;
			}

			loadedFilesPerType.Remove(fileType);

			OnAllFilesOfTypeDeleted.Invoke(fileType);
		}

		/// <summary>
		/// Deletes all files of all types
		/// </summary>
		public static void DeleteAllData()
		{
			loadedFilesPerType.Clear();
			currentFileIndexPerType.Clear();

			FileParser.DeleteAllData();

			OnDeletedEverything.Invoke();
		}
	}
}