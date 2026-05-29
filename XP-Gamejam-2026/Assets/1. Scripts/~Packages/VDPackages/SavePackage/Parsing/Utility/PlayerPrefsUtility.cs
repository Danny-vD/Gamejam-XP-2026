using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VDPackages.SavePackage.FileManagement;

namespace VDPackages.SavePackage.Parsing.Utility
{
	/// <summary>
	/// Helps with retrieving all the 'files' saved in PlayerPrefs through the <see cref="FileManager"/>
	/// </summary>
	public static class PlayerPrefsUtility
	{
		/// <summary>
		/// The player prefs key used to store the names of all save files in a '\n'-seperated list
		/// </summary>
		public const string PLAYER_PREFS_FILES_KEY = "FileKeys";

		private static string[] cachedFilesArray = Array.Empty<string>();
		
		public static string[] GetFiles()
		{
			if (cachedFilesArray.Length == 0 && PlayerPrefs.HasKey(PLAYER_PREFS_FILES_KEY))
			{
				cachedFilesArray = PlayerPrefs.GetString(PLAYER_PREFS_FILES_KEY).Split('\n');
			}

			return cachedFilesArray;
		}

		public static void AddFileKey(string fileKey)
		{
			if (GetFiles().Contains(fileKey))
			{
				return;
			}

			ResetCachedArray();

			if (!PlayerPrefs.HasKey(PLAYER_PREFS_FILES_KEY))
			{
				PlayerPrefs.SetString(PLAYER_PREFS_FILES_KEY, fileKey);
				return;
			}

			string saveFileNames = PlayerPrefs.GetString(PLAYER_PREFS_FILES_KEY);

			string stringToAdd = "\n" + fileKey;
			saveFileNames += stringToAdd;

			PlayerPrefs.SetString(PLAYER_PREFS_FILES_KEY, saveFileNames);
		}

		public static void DeleteSaveFileKey(string saveFileName)
		{
			if (!PlayerPrefs.HasKey(PLAYER_PREFS_FILES_KEY))
			{
				return;
			}

			string saveFileNames = PlayerPrefs.GetString(PLAYER_PREFS_FILES_KEY);

			string stringToRemove = saveFileName;
			int indexOfRemoval = saveFileNames.IndexOf(stringToRemove, StringComparison.InvariantCulture);

			if (indexOfRemoval != -1 && indexOfRemoval != 0)
			{
				stringToRemove =  "\n" + saveFileName;
				indexOfRemoval -= 1; // Start removing 1 character earlier to also remove the newline char
			}
			
			if (indexOfRemoval == -1)
			{
				return;
			}

			saveFileNames = saveFileNames.Remove(indexOfRemoval, stringToRemove.Length);
			ResetCachedArray();

			if (string.IsNullOrEmpty(saveFileNames))
			{
				DeleteInternalKeys();
				return;
			}

			PlayerPrefs.SetString(PLAYER_PREFS_FILES_KEY, saveFileNames);
		}

		public static void DeleteSaveFileDirectory(string directoryPath)
		{
			if (!PlayerPrefs.HasKey(PLAYER_PREFS_FILES_KEY))
			{
				return;
			}

			string[] saveFiles = GetFiles();

			List<string> newSaveNames = new List<string>();

			foreach (string saveFilePath in saveFiles)
			{
				if (saveFilePath.StartsWith(directoryPath))
				{
					PlayerPrefs.DeleteKey(saveFilePath);
				}
				else
				{
					newSaveNames.Add(saveFilePath);
				}
			}

			PlayerPrefs.SetString(PLAYER_PREFS_FILES_KEY, string.Join('\n', newSaveNames));
			ResetCachedArray();
		}

		public static void DeleteAllSaves()
		{
			foreach (string saveFile in GetFiles())
			{
				PlayerPrefs.DeleteKey(saveFile);
			}

			DeleteInternalKeys();
			ResetCachedArray();
		}

		private static void ResetCachedArray()
		{
			cachedFilesArray = Array.Empty<string>();
		}

		private static void DeleteInternalKeys()
		{
			PlayerPrefs.DeleteKey(PLAYER_PREFS_FILES_KEY);
		}
	}
}