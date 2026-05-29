using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using VDPackages.SavePackage.FileManagement;
using VDPackages.SavePackage.SavableFiles.BaseClasses;

namespace VDPackages.SavePackage.Utility
{
	public static class CurrentFileHelper
	{
		private static readonly Dictionary<Type, Action> dataChangedEventPerType = new Dictionary<Type, Action>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			FileManager.OnCurrentFileChanged += OnCurrentFileChanged;

			Application.quitting += Finalise;
		}

		private static void Finalise()
		{
			FileManager.OnCurrentFileChanged -= OnCurrentFileChanged;

			Application.quitting -= Finalise;
		}

		public static TFileType GetCurrentFile<TFileType>() where TFileType : AbstractSavableFile
		{
			//TODO: There are still some configFile == null checks all over the place after GetCurrentFile, these should be removed
			TFileType currentFile = FileManager.GetCurrentFile<TFileType>();

			if (currentFile == null)
			{
				throw new DataException($"There is no file of type {typeof(TFileType)} set!\nEnsure there is one before you attempt to read/write to it!");
			}

			return currentFile;
		}

		public static void AddListenerToDataChanged(Type fileType, Action function)
		{
			AbstractSavableFile currentFile = FileManager.GetCurrentFile(fileType);

			bool hasCurrentFile = currentFile != null;

			if (!dataChangedEventPerType.TryAdd(fileType, null)) // Returns false if the key already exists
			{
				if (hasCurrentFile) // If has current file and existing listeners, remove them all
				{
					if (dataChangedEventPerType[fileType] != null)
					{
						currentFile.OnDataChanged -= dataChangedEventPerType[fileType].Invoke;
					}
				}
			}

			dataChangedEventPerType[fileType] += function; // Update the listeners to invoke

			if (hasCurrentFile)
			{
				currentFile.OnDataChanged += dataChangedEventPerType[fileType].Invoke; // Add all listeners to DataChanged (this is necessary because the Action is now a different object)
			}
		}

		public static void RemoveListenerToDataChanged(Type fileType, Action function)
		{
			if (!dataChangedEventPerType.ContainsKey(fileType))
			{
				return;
			}

			AbstractSavableFile currentFile = FileManager.GetCurrentFile(fileType);
			bool hasCurrentFile = currentFile != null;

			if (hasCurrentFile) // If has current file and existing listeners, remove them all
			{
				// If you're reading this because an exception was thrown here: I promise you, the code is fine, you're just trying to remove a listener before you added it. 
				currentFile.OnDataChanged -= dataChangedEventPerType[fileType].Invoke;
			}

			dataChangedEventPerType[fileType] -= function; // Update the listeners to invoke

			if (dataChangedEventPerType[fileType] == null)
			{
				return;
			}

			if (hasCurrentFile)
			{
				currentFile.OnDataChanged += dataChangedEventPerType[fileType].Invoke; // Add all listeners to DataChanged (this is necessary because the Action is now a different object)
			}
		}

		private static void OnCurrentFileChanged(Type fileType, AbstractSavableFile previousCurrentFile, AbstractSavableFile currentFile)
		{
			if (ReferenceEquals(previousCurrentFile, currentFile)) // The file didn't actually change (this happens in the case of a reset or reload)
			{
				return;
			}

			if (!dataChangedEventPerType.ContainsKey(fileType) || dataChangedEventPerType[fileType] == null)
			{
				return;
			}

			if (previousCurrentFile != null)
			{
				previousCurrentFile.OnDataChanged -= dataChangedEventPerType[fileType].Invoke;
			}

			if (currentFile != null)
			{
				currentFile.OnDataChanged += dataChangedEventPerType[fileType].Invoke;
			}
		}
	}
}