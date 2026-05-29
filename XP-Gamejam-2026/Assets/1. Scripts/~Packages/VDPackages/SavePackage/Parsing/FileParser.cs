using System;
using System.Collections.Generic;
using VDPackages.SavePackage.Parsing.Implementations.Default;
using VDPackages.SavePackage.Parsing.Interfaces;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;

namespace VDPackages.SavePackage.Parsing
{
	/// <summary>
	/// Acts as a façade to actual implementations of IO-functions
	/// </summary>
	public static class FileParser
	{
		public static IFileParserImplementation ParserImplementation = new FileParserDefaultImplementation();

		public static IEnumerable<string> GetAllFilePaths()
		{
			return ParserImplementation.GetAllFilePaths();
		}
		
		public static MetaData[] GetAllFilesOfType(Type fileType)
		{
			IEnumerable<string> filePaths = ParserImplementation.GetAllFilePathsInTypeDirectory(fileType);

			return ParserHelper.GetMetaDatas(fileType, filePaths);
		}

		public static MetaData[] GetAllFilesOfTypeInSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true)
		{
			IEnumerable<string> filePaths = ParserImplementation.GetAllFilePathsOfTypeInSubfolder(fileType, subFolder, includeNestedFolders);
			
			return ParserHelper.GetMetaDatas(fileType, filePaths);
		}

		/// <summary>
		/// Attempt to parse the file with the given metadata
		/// </summary>
		/// <returns>The file at that location or NULL</returns>
		public static AbstractSavableFile Parse(Type fileType, MetaData fileLocation)
		{
			fileLocation.ThrowIfInvalid();
			
			return ParserImplementation.Parse(fileType, fileLocation);
		}

		public static AbstractSavableFile[] ParseAllFilesOfType(Type fileType)
		{
			MetaData[] metaInformation = GetAllFilesOfType(fileType);
			AbstractSavableFile[] saveFiles = new AbstractSavableFile[metaInformation.Length];

			for (int i = 0; i < saveFiles.Length; i++)
			{
				saveFiles[i] = Parse(fileType, metaInformation[i]);
			}

			return saveFiles;
		}
		
		public static AbstractSavableFile[] ParseAllFiles()
		{
			IEnumerable<string> filePaths = GetAllFilePaths();

			List<AbstractSavableFile> files = new List<AbstractSavableFile>();
			
			foreach (string filePath in filePaths)
			{
				if (ParserImplementation.TryGetTypeOfFile(filePath, out Type type))
				{
					MetaData metaData = MetaData.FromFilePath(type, filePath);
					AbstractSavableFile file = ParserImplementation.Parse(type, metaData);
					
					files.Add(file);
				}
			}

			return files.ToArray();
		}

		public static void Save(AbstractSavableFile file)
		{
			file.MetaData.ThrowIfInvalid();
			
			file.TimeOfLastSave = DateTime.Now;
			ParserImplementation.Save(file);

			file.IsDirty = false;
		}

		public static void MoveFile(MetaData oldMetaData, AbstractSavableFile file)
		{
			oldMetaData.ThrowIfInvalid();
			file.MetaData.ThrowIfInvalid();

			ParserImplementation.Move(oldMetaData, file);
		}

		public static bool DeleteFile(Type fileType, MetaData metaData)
		{
			metaData.ThrowIfInvalid();

			return ParserImplementation.Delete(fileType, metaData);
		}

		public static bool DeleteAllFilesOfType(Type fileType)
		{
			return ParserImplementation.DeleteAllOfType(fileType);
		}

		public static void DeleteFileDirectory(Type fileType, string directoryPath)
		{
            ParserImplementation.DeleteDirectory(fileType, directoryPath);
		}

		public static void DeleteAllData()
		{
            ParserImplementation.DeleteAllData();
		}
	}
}