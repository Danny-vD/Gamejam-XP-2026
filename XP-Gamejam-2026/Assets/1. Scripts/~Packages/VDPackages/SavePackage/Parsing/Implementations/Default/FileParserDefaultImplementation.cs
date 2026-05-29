using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VDFramework.Logger;
using VDPackages.SavePackage.Parsing.Interfaces;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.SpecificClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.Parsing.Implementations.Default
{
	/// <summary>
	/// Provides platform-specific functions for performing IO-operations on given files
	/// </summary>
	/// <unitysymbols>https://docs.unity3d.com/Manual/scripting-symbol-reference.html</unitysymbols>
	public partial class FileParserDefaultImplementation : IFileParserImplementation
	{
		private const string configurationSubFolder = "Configuration";
		private const string saveSubFolder = "Saves";

		//\\//\\//\\//\\//\\//
		// Directory paths
		//\\//\\//\\//\\//\\//

		/// <summary>
		/// Get the paths of all directories where files are saved
		/// </summary>
		public static string[] GetAllDirectoryPaths() // Public so it can be reused by other implementations | Not part of the interface but used internally
		{
			List<string> paths = new List<string>
			{
				GetDirectoryPathForType(typeof(GenericConfigFile)),
			};

#if UNITY_STANDALONE
			paths.Add(GetStandaloneSavePath());
#endif

			return paths.ToArray();
		}

		public static string GetDirectoryPathForType(Type fileType)
		{
			if (fileType == null)
			{
				return string.Empty;
			}

			Type configType = typeof(GenericConfigFile);

			if (fileType == configType || fileType.IsSubclassOf(configType)) // Config files go into streaming assets
			{
				return Path.Join(Application.streamingAssetsPath, configurationSubFolder);
			}

			if (fileType.IsSubclassOf(typeof(AbstractSavableFile))) // fallback path to automatically support all other types
			{
#if UNITY_STANDALONE
				return GetStandaloneSavePath();
#endif
			}

			return string.Empty; // Return empty string for any other types (although in practice this should never happen)
		}

		//\\//\\//\\//\\//\\//
		// File paths
		//\\//\\//\\//\\//\\//

		public IEnumerable<string> GetAllFilePaths()
		{
#if UNITY_STANDALONE
			return StandaloneGetAllFilePaths();
#else
			return PlayerPrefsGetAllSavedKeys();
#endif
		}

		public IEnumerable<string> GetAllFilePathsInTypeDirectory(Type fileType)
		{
#if UNITY_STANDALONE
			return StandaloneGetAllFilePathsInTypeDirectory(fileType);
#else
			return PlayerPrefsGetAllFilePathsInTypeDirectory(fileType);
#endif
		}

		public IEnumerable<string> GetAllFilePathsOfTypeInSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true)
		{
#if UNITY_STANDALONE
			return StandaloneGetAllFilePathsOfTypeInSubfolder(fileType, subFolder, includeNestedFolders);
#else
			return PlayerPrefsGetAllFilePathsOfTypeInSubfolder(fileType, subFolder, includeNestedFolders);
#endif
		}

		//\\//\\//\\//\\//\\//
		// Parsing
		//\\//\\//\\//\\//\\//

		public AbstractSavableFile Parse(Type fileType, MetaData metaData)
		{
			LogManager.LogInfo("Trying to load:\n" + FileDirectoryHelper.GetFullPath(fileType, metaData, false) + "\n");

#if UNITY_STANDALONE
			return StandaloneParse(fileType, metaData);
#else
			return PlayerPrefsParse(fileType, metaData);
#endif
		}

		//\\//\\//\\//\\//\\//
		// Saving
		//\\//\\//\\//\\//\\//

		public void Save(AbstractSavableFile file)
		{
			LogManager.LogInfo("Saving:\n" + file.PathOnDisk + "\n");
			
#if UNITY_STANDALONE
			StandaloneSave(file);
#else
			PlayerPrefsSave(file, true);
#endif
		}

		//\\//\\//\\//\\//\\//
		// Moving
		//\\//\\//\\//\\//\\//

		public void Move(MetaData oldMetaData, AbstractSavableFile file)
		{
#if UNITY_STANDALONE
			StandaloneMove(oldMetaData, file);
#else
			PlayerPrefsMove(oldMetaData, file);
#endif
		}

		//\\//\\//\\//\\//\\//
		// Deleting
		//\\//\\//\\//\\//\\//

		public bool Delete(Type fileType, MetaData metaData)
		{
#if UNITY_STANDALONE
			return StandaloneDelete(fileType, metaData);
#else
			return PlayerPrefsDelete(fileType, metaData, true);
#endif
		}

		public bool DeleteAllOfType(Type fileType)
		{
#if UNITY_STANDALONE
			return StandaloneDeleteAllOfType(fileType);
#else
			return PlayerPrefsDeleteAllOfType(fileType);
#endif
		}

		public void DeleteDirectory(Type fileType, string directoryPath)
		{
#if UNITY_STANDALONE
			StandaloneDeleteDirectory(fileType, directoryPath);
#else
			PlayerPrefsDeleteDirectory(fileType, directoryPath);
#endif
		}

		public void DeleteAllData()
		{
#if UNITY_STANDALONE
			StandaloneDeleteAll();
#else
			PlayerPrefsDeleteAll();
#endif
		}

		//\\//\\//\\//\\//\\//
		// Utility
		//\\//\\//\\//\\//\\//

		public bool TryGetTypeOfFile(string filePath, out Type fileType)
		{
#if UNITY_STANDALONE
			return StandaloneTryGetTypeOfFile(filePath, out fileType);
#else
			return PlayerPrefsTryGetTypeOfFile(filePath, out fileType);
#endif
		}
		
		//\\//\\//\\//\\//\\//
		// Internal
		//\\//\\//\\//\\//\\//

		// Deletes the root company directory so no lingering empty directories remain
		private static void RemoveCompanyDirectoryIfEmpty()
		{
			foreach (string directoryPath in GetAllDirectoryPaths())
			{
				if (!directoryPath.EndsWith(FileDirectoryHelper.ApplicationSubFolder))
				{
					continue;
				}

				string companyDirectory = Path.GetDirectoryName(directoryPath);

				if (string.IsNullOrEmpty(companyDirectory) || !Directory.Exists(companyDirectory))
				{
					continue;
				}

				if (Directory.GetFiles(companyDirectory, "*", SearchOption.AllDirectories).Length == 0)
				{
					Directory.Delete(companyDirectory, true);
				}
			}
		}
	}
}