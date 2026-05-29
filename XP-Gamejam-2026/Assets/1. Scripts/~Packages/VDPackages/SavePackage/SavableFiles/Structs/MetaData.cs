using System;
using System.IO;
using VDFramework.Logger;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.SavableFiles.Structs
{
	/// <summary>
	/// Contains extra information about the file itself, like the FileName and the subfolders it is in
	/// </summary>
	[Serializable]
	public struct MetaData
	{
		/// <summary>
		/// Interprets a string that represents a path and automatically fills in the respective fields in a <see cref="MetaData"/> object
		/// </summary>
		public static MetaData FromFilePath(Type fileType, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return new MetaData();
			}

			string saveDirectoryPath = FileDirectoryHelper.GetRootDirectoryPathForType(fileType, false);

			if (!string.IsNullOrEmpty(saveDirectoryPath)) // Cut off the part of the path that leads to the save directory
			{
				int startOfPathIndex = saveDirectoryPath.Length + 1; // + 1 because there's a directory seperator between the root and the current path

				if (startOfPathIndex >= path.Length)
				{
					LogManager.LogError($"There is a discreprancy in the file paths!\nThis is caused by the path returned in {nameof(FileDirectoryHelper)} not matching the actual path where it is saved!\n\nGiven Path:\n{path}\n\nExpected root:\n{saveDirectoryPath}\n");
				}
				else
				{
					path = path.Substring(startOfPathIndex);
				}
			}

			string subFolders = Path.GetDirectoryName(path) ?? string.Empty; // Returns null if there is no directory seperator char

			string fileName = Path.GetFileNameWithoutExtension(path);
			string extension = Path.GetExtension(path);

			return new MetaData(fileName, extension, subFolders);
		}

		/// <summary>
		/// Any additional folders between this file and the root save file directory
		/// </summary>
		/// <remarks>Should neither start nor end with <see cref="Path.DirectorySeparatorChar"/> or <see cref="Path.AltDirectorySeparatorChar"/></remarks>
		public string SubFolders;

		/// <summary>
		/// The name of the file
		/// </summary>
		public string FileName;

		/// <summary>
		/// The extension of the file
		/// </summary>
		/// <remarks>Has to start with a '.', one will be added at the start if it is not present</remarks>
		public string Extension;

		public MetaData(string fileName) : this(fileName, string.Empty, string.Empty)
		{
		}

		public MetaData(string fileName, string extension) : this(fileName, extension, string.Empty)
		{
		}

		public MetaData(string fileName, string extension, string subFolders)
		{
			SubFolders = subFolders;
			FileName  = fileName;
			Extension = extension;
			
			StandardiseSubFolders();
		}

		/// <summary>
		/// Returns the combination of subfolders, the name of the file and the extension (if present)
		/// </summary>
		/// <returns>[<see cref="SubFolders"/>]/[<see cref="FileName"/>].[<see cref="Extension"/>]</returns>
		/// <remarks>Only contains directorySeperatorChars if there are subfolders</remarks>
		/// <seealso cref="FileDirectoryHelper.GetFullPath"/>
		public string GetSubPath()
		{
			return Path.Join(GetSubFolders(), FileName + GetExtension());
		}

		/// <summary>
		/// Get the <see cref="SubFolders"/> with extra enforcement regarding not starting and ending with a directory seperator char and directory seperator characters
		/// </summary>
		public string GetSubFolders()
		{
			if (string.IsNullOrEmpty(SubFolders))
			{
				return string.Empty;
			}

			StandardiseSubFolders();

			return SubFolders;
		}

		/// <summary>
		/// Get the <see cref="Extension"/> with extra enforcement regarding not starting with a '.'
		/// </summary>
		public string GetExtension()
		{
			if (string.IsNullOrEmpty(Extension))
			{
				return string.Empty;
			}

			if (Extension.StartsWith('.'))
			{
				return Extension;
			}

			return "." + Extension;
		}

		/// <summary>
		/// <para>A file is valid if it has a <see cref="FileName"/> and there are no illegal characters in the SubFolders or Filename.</para>
		/// <para><see cref="SubFolders"/> and <see cref="Extension"/>s are optional (may be empty).</para>
		/// </summary>
		/// <seealso cref="Path.GetInvalidFileNameChars()"/>
		/// <seealso cref="Path.GetInvalidPathChars()"/>
		public bool IsValid()
		{
			return IsValidFileName() && IsValidSubFolderPath();
		}

		private bool IsValidFileName()
		{
			if (string.IsNullOrEmpty(FileName))
			{
				return false;
			}

			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

			int indexOfAny = FileName.IndexOfAny(invalidFileNameChars);

			if (indexOfAny != -1)
			{
				// Filename contains invalid characters
				return false;
			}

			return true;
		}

		private bool IsValidSubFolderPath()
		{
			if (string.IsNullOrEmpty(SubFolders))
			{
				return true; // SubFolder is allowed to be empty
			}

			char[] invalidPathChars = Path.GetInvalidPathChars();

			int indexOfAny = SubFolders.IndexOfAny(invalidPathChars);

			if (indexOfAny != -1)
			{
				// Subfolder string contains invalid characters
				return false;
			}

			return true;
		}

		/// <summary>
		/// Throws an exception if the current state of the MetaData is not valid
		/// </summary>
		/// <seealso cref="IsValid"/>
		public void ThrowIfInvalid()
		{
			if (!IsValidFileName())
			{
				throw new InvalidDataException($"The filename is either empty or contains invalid characters!\nInvalid characters: {string.Join(" ", Path.GetInvalidFileNameChars())}\nValue: \"{FileName}\"");
			}

			if (!IsValidSubFolderPath())
			{
				throw new InvalidDataException($"The Subfolder path contains invalid characters!\nInvalid characters: {string.Join(" ", Path.GetInvalidPathChars())}\nValue: \"{SubFolders}\"");
			}
		}

		/// <summary>
		/// Standardise the subfolders by ensuring they neither start nor end with a DirectorySeperatorCharacter and by replacing all the ALT seperator characters by the normal seperator character
		/// </summary>
		private void StandardiseSubFolders()
		{
			// Subfolders shouldn't start with a seperator char
			SubFolders = SubFolders.Trim(PathConstants.DirectorySeperatorCharacters);
			
			if (Path.AltDirectorySeparatorChar != Path.DirectorySeparatorChar)
			{
				// Standardise the subfolders by replacing the alt directory char with the normal seperator char (if they're different)
				SubFolders = SubFolders.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
		}
		
		public override string ToString()
		{
			return GetSubPath();
		}
	}
}