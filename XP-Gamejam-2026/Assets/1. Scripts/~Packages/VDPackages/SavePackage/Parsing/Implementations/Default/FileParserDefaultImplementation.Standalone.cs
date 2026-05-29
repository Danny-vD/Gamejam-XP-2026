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
		// Directory paths
		//\\//\\//\\//\\//\\//

		private static string GetStandaloneSavePath()
		{
#if UNITY_STANDALONE_WIN // Use the SavedGames folder on windows (the 'official' folder for saved games)
			try
			{
				// Throws if the folder does not exist
				return Path.Join(VDFramework.Utility.Windows.SpecialFolders.SavedGames.Path, FileDirectoryHelper.ApplicationSubFolder);
			}
			catch (DirectoryNotFoundException)
			{
				return Path.Join(VDFramework.Utility.Windows.SpecialFolders.SavedGames.DefaultPath, FileDirectoryHelper.ApplicationSubFolder);
			}
#endif

			return Path.Join(Application.streamingAssetsPath, saveSubFolder);
		}

		//\\//\\//\\//\\//\\//
		// File paths
		//\\//\\//\\//\\//\\//

		private static IEnumerable<string> StandaloneGetAllFilePaths()
		{
			foreach (string directoryPath in GetAllDirectoryPaths())
			{
				if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
				{
					continue;
				}

				foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
				{
					yield return filePath;
				}
			}
		}

		private static IEnumerable<string> StandaloneGetAllFilePathsInTypeDirectory(Type fileType)
		{
			string directoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);

			if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
			{
				return Enumerable.Empty<string>();
			}

			return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories);
		}

		private static IEnumerable<string> StandaloneGetAllFilePathsOfTypeInSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true)
		{
			string directoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);
			directoryPath = Path.Join(directoryPath, subFolder);

			if (!Directory.Exists(directoryPath))
			{
				return Enumerable.Empty<string>();
			}

			return Directory.EnumerateFiles(directoryPath, "*", includeNestedFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		//\\//\\//\\//\\//\\//
		// Parsing
		//\\//\\//\\//\\//\\//

		private static AbstractSavableFile StandaloneParse(Type fileType, MetaData metaData)
		{
			string filePath = FileDirectoryHelper.GetFullPath(fileType, metaData, false);

			if (!File.Exists(filePath))
			{
				return null;
			}

			string json = File.ReadAllText(filePath);

			AbstractSavableFile file = ParserHelper.FromJson(fileType, json);
			file.SetMetaData(metaData);

			return file;
		}

		//\\//\\//\\//\\//\\//
		// Saving
		//\\//\\//\\//\\//\\//

		private static void StandaloneSave(AbstractSavableFile file)
		{
			string filePath = FileDirectoryHelper.GetFullPath(file.GetType(), file.MetaData, true);

			string json = ParserHelper.ToJson(file, true);

			File.WriteAllText(filePath, json);
		}

		//\\//\\//\\//\\//\\//
		// Moving
		//\\//\\//\\//\\//\\//

		private static void StandaloneMove(MetaData oldMetaData, AbstractSavableFile file)
		{
			Type fileType = file.GetType();

			string oldFilePath = FileDirectoryHelper.GetFullPath(fileType, oldMetaData, false);

			if (!File.Exists(oldFilePath))
			{
				// The idea of a move is that the file ends up in the target location, if it was not present in the previous location save it to the target to ensure this
				StandaloneSave(file);
				return;
			}

			string filePath = FileDirectoryHelper.GetFullPath(fileType, file.MetaData, true);

			File.Delete(filePath); // The overwrite overload does not exist for File.Move (2026-02-19) so we explicitly delete the file if it exists in the new location
			File.Move(oldFilePath, filePath);
		}

		//\\//\\//\\//\\//\\//
		// Deleting
		//\\//\\//\\//\\//\\//

		private static bool StandaloneDelete(Type fileType, MetaData metaData)
		{
			string filePath = FileDirectoryHelper.GetFullPath(fileType, metaData, false);

			string directoryPath = Path.GetDirectoryName(filePath);

			if (Directory.Exists(directoryPath) && File.Exists(filePath))
			{
				File.Delete(filePath); // Does not throw if the file does not exist
				return true;
			}

			return false;
		}

		private static void StandaloneDeleteDirectory(Type fileType, string directoryPath)
		{
			string rootDirectoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);
			string path = Path.Join(rootDirectoryPath, directoryPath);

			Directory.Delete(path, true); // Does nothing if the path is invalid
		}

		private static bool StandaloneDeleteAllOfType(Type fileType)
		{
			IEnumerable<string> filePathsOfType = ParserHelper.FilterByType(fileType, StandaloneGetAllFilePathsInTypeDirectory(fileType)).ToArray();

			bool anyDeleted = false;

			foreach (string filePath in filePathsOfType)
			{
				string directoryPath = Path.GetDirectoryName(filePath);

				if (!Directory.Exists(directoryPath))
				{
					continue;
				}

				if (!File.Exists(filePath))
				{
					continue;
				}

				File.Delete(filePath); // Does not throw if the file does not exist
				anyDeleted = true;
			}

			return anyDeleted;
		}

		private static void StandaloneDeleteAll()
		{
			foreach (string directoryPath in GetAllDirectoryPaths())
			{
				if (Directory.Exists(directoryPath))
				{
					Directory.Delete(directoryPath, true);
				}
			}

			RemoveCompanyDirectoryIfEmpty();
		}

		//\\//\\//\\//\\//\\//
		// Utility
		//\\//\\//\\//\\//\\//

		private static bool StandaloneTryGetTypeOfFile(string filePath, out Type fileType)
		{
			if (!File.Exists(filePath))
			{
				fileType = default;
				return false;
			}

			string json = File.ReadAllText(filePath);

			return ParserHelper.JsonTryGetTypeFromFile(json, out fileType);
		}
	}
}