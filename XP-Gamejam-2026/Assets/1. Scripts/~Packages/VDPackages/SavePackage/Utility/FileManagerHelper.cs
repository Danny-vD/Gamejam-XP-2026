using System;
using UnityEngine;
using VDPackages.SavePackage.FileManagement;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;

namespace VDPackages.SavePackage.Utility
{
	public static class FileManagerHelper
	{
		/// <summary>
		/// <para>Ensure there is always a current file by creating one if none exists and setting current-index to 0</para>
		/// <para>If any file with of the given type and metadata is saved to disk, then it will be loaded and set as the current file</para>
		/// <para>If that file is not present, then the first one saved to disk is loaded and set as current</para>
		/// <para>If there already is a valid file set, then nothing will happen</para>
		/// </summary>
		public static void LoadOrCreateFileIfNoCurrentAndSetAsCurrent<TFileType>(MetaData metaData) where TFileType : AbstractSavableFile, new()
		{
			Type fileType = typeof(TFileType);

			if (FileManager.GetCurrentFile(fileType) != null)
			{
				return;
			}
			
			LoadOrCreateFile<TFileType>(metaData, out int index);

			FileManager.SetCurrentFileToIndex(fileType, index, false);
		}

		/// <summary>
		/// Create a new file and make set it as the 'current' of that type
		/// </summary>
		public static void CreateNewFileAndSetAsCurrent<TFileType>(MetaData metaData) where TFileType : AbstractSavableFile, new()
		{
			int index = FileManager.CreateNewFileOfType(metaData, out TFileType _);
			FileManager.SetCurrentFileToIndex<TFileType>(index, true);
		}

		public static TFileType LoadOrCreateFile<TFileType>(MetaData metaData, out int index) where TFileType : AbstractSavableFile, new()
		{
			TFileType file = FileManager.LoadFile<TFileType>(metaData, out index);

			if (file == null)
			{
			 	index = FileManager.CreateNewFileOfType(metaData, out file);
			}

			return file;
		}
		
		/// <summary>
		/// Get the file with the given <see cref="MetaData"/> if it's loaded, otherwise try to load it.
		/// </summary>
		public static bool TryGetFile(Type fileType, MetaData metaData, out int index, out AbstractSavableFile file)
		{
			index = FileManager.GetIndexOfFirstMatchingFile(fileType, file => file.MetaData.Equals(metaData), out file);

			if (index != -1)
			{
				return true;
			}
			
			file = FileManager.LoadFile(fileType, metaData, out index);

			return index >= 0;
		}
	}
}