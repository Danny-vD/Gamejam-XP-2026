using System;
using System.Collections.Generic;
using VDPackages.SavePackage.Parsing.Implementations.Default;
using VDPackages.SavePackage.Parsing.Utility;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.Parsing.Interfaces
{
	/// <summary>
	/// Implements IO functions to perform operations on files
	/// </summary>
	public interface IFileParserImplementation
	{
		//\\//\\//\\//\\//\\//
		// Directory paths
		//\\//\\//\\//\\//\\//
		
		/// <summary>
		/// <para>The function used internally by the <see cref="FileDirectoryHelper"/> to get the path for any given type</para>
		/// <para>Defaults to <see cref="FileParserDefaultImplementation.GetDirectoryPathForType">FileParserDefaultImplementation.GetDirectoryPathForType</see></para>
		/// </summary>
		public string GetDirectoryPathForType(Type fileType)
		{
			return FileParserDefaultImplementation.GetDirectoryPathForType(fileType);
		}
		
		//\\//\\//\\//\\//\\//
		// File paths
		//\\//\\//\\//\\//\\//
        
		public IEnumerable<string> GetAllFilePaths();

		/// <summary>
		/// Returns all the files (full path) in the save directory of the given type
		/// </summary>
		/// <param name="fileType">The type of the file</param>
		/// <returns>All the file paths in the directory for that type</returns>
		/// <remarks>Implementors do not have to manually filter which of the files are actually of the given type, that will be done by the <see cref="FileParser"/></remarks>
		/// <seealso cref="ParserHelper.FilterByType"/>
		public IEnumerable<string> GetAllFilePathsInTypeDirectory(Type fileType);

		/// <summary>
		/// Returns all the files (full path) in the a subfolder of the save directory of the given type 
		/// </summary>
		/// <param name="fileType">The type of the file</param>
		/// <returns>All the file paths in the subfolder of the directory for that type</returns>
		/// <remarks>Implementors do not have to manually filter which of the files are actually of the given type, that will be done by the <see cref="FileParser"/></remarks>
		/// <seealso cref="ParserHelper.FilterByType"/>
		public IEnumerable<string> GetAllFilePathsOfTypeInSubfolder(Type fileType, string subFolder, bool includeNestedFolders = true);

		//\\//\\//\\//\\//\\//
		// Parsing
		//\\//\\//\\//\\//\\//
		
		/// <summary>
		/// Attempt to parse the file with the given metadata
		/// </summary>
		/// <returns>The file at the given location or NULL</returns>
		public AbstractSavableFile Parse(Type fileType, MetaData metaData);

		//\\//\\//\\//\\//\\//
		// Saving
		//\\//\\//\\//\\//\\//
		
		/// <summary>
		/// <para>Save the given file to disk</para>
		/// <para>The type can be retrieved from the file if needed</para>
		/// </summary>
		/// <remarks>The type can be retrieved from the file if needed</remarks>
		public void Save(AbstractSavableFile file);

		//\\//\\//\\//\\//\\//
		// Moving
		//\\//\\//\\//\\//\\//

		/// <summary>
		/// Move the file with the given metadata to the location of the metadata of <paramref name="file"/>
		/// </summary>
		/// <remarks>The type can be retrieved from the file if needed</remarks>
		public void Move(MetaData oldMetaData, AbstractSavableFile file);

		//\\//\\//\\//\\//\\//
		// Deleting
		//\\//\\//\\//\\//\\//

		public bool Delete(Type fileType, MetaData metaData);

		public bool DeleteAllOfType(Type fileType);

		public void DeleteDirectory(Type fileType, string directoryPath);

		public void DeleteAllData();

		//\\//\\//\\//\\//\\//
		// Utility
		//\\//\\//\\//\\//\\//
		
		/// <summary>
		/// Deserialise the minimum amount of the given file and get the type that it belongs to (or NULL if it is invalid)
		/// </summary>
		public bool TryGetTypeOfFile(string filePath, out Type fileType);
	}
}