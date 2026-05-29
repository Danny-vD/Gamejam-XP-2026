using System;
using System.IO;
using UnityEngine;
using VDPackages.SavePackage.Parsing;
using VDPackages.SavePackage.SavableFiles.Structs;

namespace VDPackages.SavePackage.SaveDirectory
{
	/// <summary>
	/// A helper class used for getting the path to where files would be stored
	/// </summary>
	public static class FileDirectoryHelper
	{
		/// <summary>
		/// A subpath that contains both the company name and product name as set in player settings
		/// </summary>
		/// <code>Path.Join(Application.companyName, Application.productName);</code>
		public static readonly string ApplicationSubFolder = Path.Join(Application.companyName, Application.productName);
		
		/// <summary>
		/// <para>The path to the directory where all the files of the given type are stored or empty string if not applicable for the current platform/file</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		public static string GetRootDirectoryPathForType(Type fileType, bool createDirectoryIfNotExist)
		{
			string path = FileParser.ParserImplementation.GetDirectoryPathForType(fileType);

			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return createDirectoryIfNotExist ? CreateDirectoryIfNotExist(path) : path;
		}

		/// <summary>
		/// <para>The path to the directory of the given file</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		public static string GetDirectoryPathForFile(Type fileType, MetaData metaData, bool createDirectoryIfNotExist)
		{
			string saveDirectoryPath = GetRootDirectoryPathForType(fileType, false);

			string path = Path.Join(saveDirectoryPath, metaData.GetSubFolders());

			if (!createDirectoryIfNotExist || !metaData.IsValid())
			{
				return path;
			}

			Directory.CreateDirectory(path);

			return path;
		}

		/// <summary>
		/// <para>Get the path to this file, either as an absolute path or only the <see cref="MetaData.GetSubPath"/> if there is no directory path for the given type</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		public static string GetFullPath(Type fileType, MetaData metaData, bool createDirectoryIfNotExist)
		{
			string saveDirectoryPath = GetRootDirectoryPathForType(fileType, false);

			string path = Path.Join(saveDirectoryPath, metaData.GetSubPath());

			if (!createDirectoryIfNotExist || !metaData.IsValid())
			{
				return path;
			}

			string directoryPath = Path.GetDirectoryName(path); // Returns null if there is no directory seperator char

			if (directoryPath != null)
			{
				Directory.CreateDirectory(directoryPath);
			}

			return path;
		}

		/// <summary>
		/// <para>The path to the directory where all the files of the given type are stored as an absolute path.</para>
		/// <para>If the path returned by <see cref="GetRootDirectoryPathForType"/> is a relative path, the current working directory is prepended to it.</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		/// <seealso cref="Directory.GetCurrentDirectory"/>
		public static string GetAbsoluteFileDirectoryPathForType(Type fileType, bool createDirectoryIfNotExist)
		{
			string path = GetRootDirectoryPathForType(fileType, false);

			if (!Path.IsPathFullyQualified(path))
			{
				path = Path.Join(Directory.GetCurrentDirectory(), path);
			}

			if (createDirectoryIfNotExist)
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		/// <summary>
		/// <para>The path to the directory of the given file</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		public static string GetAbsoluteDirectoryPathForFile(Type fileType, MetaData metaData, bool createDirectoryIfNotExist)
		{
			string saveDirectoryPath = GetAbsoluteFileDirectoryPathForType(fileType, false);

			string path = Path.Join(saveDirectoryPath, metaData.GetSubFolders());

			if (!createDirectoryIfNotExist || !metaData.IsValid())
			{
				return path;
			}

			Directory.CreateDirectory(path);

			return path;
		}

		/// <summary>
		/// <para>The path to the given file as an absolute path.</para>
		/// <para>If the path returned by <see cref="GetRootDirectoryPathForType"/> is a relative path, the current working directory is prepended to it</para>
		/// <para>Optionally creates the path to the directory if it does not exist yet</para>
		/// </summary>
		/// <remarks>The file might not exist on the given path if it was never saved yet</remarks>
		/// <seealso cref="Directory.GetCurrentDirectory"/>
		public static string GetAbsolutePathToFile(Type fileType, MetaData metaData, bool createDirectoryIfNotExist)
		{
			string saveDirectoryPath = GetAbsoluteFileDirectoryPathForType(fileType, false);

			string path = Path.Join(saveDirectoryPath, metaData.GetSubPath());

			if (!createDirectoryIfNotExist || !metaData.IsValid())
			{
				return path;
			}

			string directoryPath = Path.GetDirectoryName(path); // Returns null if there is no directory seperator char

			if (directoryPath != null)
			{
				Directory.CreateDirectory(directoryPath);
			}

			return path;
		}

		/// <summary>
		/// Creates the directory at the given path if it does not exist yet
		/// </summary>
		/// <param name="path">The path to the directory</param>
		/// <returns><paramref name="path"/>, so it's possible to use this function as an argument to another function</returns>
		public static string CreateDirectoryIfNotExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}
	}
}