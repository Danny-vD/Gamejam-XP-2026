using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.Parsing.Implementations.Default
{
	public partial class FileParserDefaultImplementation
	{
		//\\//\\//\\//\\//\\//
		// File paths
		//\\//\\//\\//\\//\\//
		
		private static IEnumerable<string> PlayerPrefsGetAllSavedKeys()
		{
			return PlayerPrefsUtility.GetFiles();
		}
		
		private static IEnumerable<string> PlayerPrefsGetAllFilePathsInTypeDirectory(Type fileType)
		{
			string[] filePaths = PlayerPrefsUtility.GetFiles();

			if (filePaths.Length == 0) // No files
			{
				return filePaths;
			}
			
			string saveDirectoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);

			return filePaths.Where(path => path.StartsWith(saveDirectoryPath));
		}

		private static IEnumerable<string> PlayerPrefsGetAllFilePathsOfTypeInSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true)
		{
			string[] filePaths = PlayerPrefsUtility.GetFiles();

			if (filePaths.Length == 0) // No files
			{
				return filePaths;
			}

			string saveDirectoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);
			string pathWithSubfolder = Path.Join(saveDirectoryPath, subFolder);

			if (includeNestedFolders)
			{
				return filePaths.Where(path => path.StartsWith(pathWithSubfolder));
			}

			// Only include the strings where the subfolder exactly matches the requested folder
			return filePaths.Where(path =>
			{
				int index = path.LastIndexOfAny(PathConstants.DirectorySeperatorCharacters);
				string subFolders = path.Substring(0, index);
				return subFolders == pathWithSubfolder;
			});
		}
		
		//\\//\\//\\//\\//\\//
		// Parsing
		//\\//\\//\\//\\//\\//

		private static AbstractSavableFile PlayerPrefsParse(Type fileType, MetaData metaData)
		{
			string saveFileKey = FileDirectoryHelper.GetFullPath(fileType, metaData, false);

			if (!PlayerPrefs.HasKey(saveFileKey))
			{
				return null;
			}
			
			AbstractSavableFile savableFile = ParserHelper.FromJson(fileType, PlayerPrefs.GetString(saveFileKey));;
			savableFile.SetMetaData(metaData);

			return savableFile;
		}
		
		//\\//\\//\\//\\//\\//
		// Saving
		//\\//\\//\\//\\//\\//

		private static void PlayerPrefsSave(AbstractSavableFile file, bool savePlayerPrefs)
		{
			string saveFileKey = file.PathOnDisk;

			PlayerPrefs.SetString(saveFileKey, ParserHelper.ToJson(file, false));
			PlayerPrefsUtility.AddFileKey(saveFileKey);

			if (savePlayerPrefs)
			{
				PlayerPrefs.Save();
			}
		}
		
		//\\//\\//\\//\\//\\//
		// Moving
		//\\//\\//\\//\\//\\//

		private static void PlayerPrefsMove(MetaData oldMetaData, AbstractSavableFile savableFile)
		{
			PlayerPrefsDelete(savableFile.GetType(), oldMetaData, false);
			PlayerPrefsSave(savableFile, true);
		}
		
		//\\//\\//\\//\\//\\//
		// Deleting
		//\\//\\//\\//\\//\\//

		private static bool PlayerPrefsDelete(Type fileType, MetaData metaData, bool savePlayerPrefs)
		{
			string saveFileKey = FileDirectoryHelper.GetFullPath(fileType, metaData, false);

			if (!PlayerPrefs.HasKey(saveFileKey))
			{
				return false;
			}

			PlayerPrefs.DeleteKey(saveFileKey);
			PlayerPrefsUtility.DeleteSaveFileKey(saveFileKey);

			if (savePlayerPrefs)
			{
				PlayerPrefs.Save();
			}

			return true;
		}

		private static void PlayerPrefsDeleteDirectory(Type fileType, string directoryPath)
		{
			string rootDirectoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);

			string path = Path.Join(rootDirectoryPath, directoryPath);

			PlayerPrefsUtility.DeleteSaveFileDirectory(path);
			PlayerPrefs.Save();
		}
		
		private static bool PlayerPrefsDeleteAllOfType(Type fileType)
		{
			IEnumerable<string> filePaths = PlayerPrefsGetAllFilePathsInTypeDirectory(fileType);
			MetaData[] metaDatas = ParserHelper.GetMetaDatas(fileType, filePaths);
			
			bool anyDeleted = false;

			foreach (MetaData metaData in metaDatas)
			{
				if (PlayerPrefsDelete(fileType, metaData, false))
				{
					anyDeleted = true;
				}
			}
			
			PlayerPrefs.Save();
			return anyDeleted;
		}

		private static void PlayerPrefsDeleteAll()
		{
			PlayerPrefsUtility.DeleteAllSaves();
			PlayerPrefs.Save();
		}

		//\\//\\//\\//\\//\\//
		// Utility
		//\\//\\//\\//\\//\\//

		private static bool PlayerPrefsTryGetTypeOfFile(string filePath, out Type fileType)
		{
			if (!PlayerPrefs.HasKey(filePath))
			{
				fileType = default;
				return false;
			}

			string json = PlayerPrefs.GetString(filePath);

			return ParserHelper.JsonTryGetTypeFromFile(json, out fileType);
		}
	}
}