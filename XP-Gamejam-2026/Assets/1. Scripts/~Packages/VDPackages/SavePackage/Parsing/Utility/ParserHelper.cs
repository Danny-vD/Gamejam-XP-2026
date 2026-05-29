using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;

namespace VDPackages.SavePackage.Parsing.Utility
{
	public static class ParserHelper
	{
		public static string ToJson(AbstractSavableFile file, bool prettyPrint)
		{
			return JsonUtility.ToJson(file, prettyPrint);
		}
		
		public static AbstractSavableFile FromJson(Type fileType, string json)
		{
			return (AbstractSavableFile)JsonUtility.FromJson(json, fileType);
		}

		public static bool JsonTryGetTypeFromFile(string json, out Type fileType)
		{
			FileTypeHolderBase typeHolder = JsonUtility.FromJson<FileTypeHolderBase>(json);

			return TypeFromStringHelper.TryGetType(typeHolder.TypeString, out fileType);
		}
		
		public static MetaData[] GetMetaDatas(Type fileType, IEnumerable<string> filePaths)
		{
			filePaths = FilterByType(fileType, filePaths);

			return filePaths.Select(path => MetaData.FromFilePath(fileType, path)).ToArray();
		}
		
		/// <summary>
		/// Filters the filepaths to files which actually match the given type
		/// </summary>
		public static IEnumerable<string> FilterByType(Type targetFileType, IEnumerable<string> filePaths)
		{
			return filePaths.Where(path => FileMatchesType(targetFileType, path));
		}

		private static bool FileMatchesType(Type targetFileType, string filePath)
		{
			if (!FileParser.ParserImplementation.TryGetTypeOfFile(filePath, out Type type))
			{
				return false;
			}

			return targetFileType == type;
		}
	}
}