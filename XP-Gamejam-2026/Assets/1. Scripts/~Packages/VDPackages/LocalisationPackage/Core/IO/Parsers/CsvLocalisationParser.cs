using System;
using System.Collections.Generic;
using UnityEngine;
using VDFramework.Logger;
using VDPackages.LocalisationPackage.Core.Enums;
using VDPackages.LocalisationPackage.Core.IO.Parsers.Interfaces;

namespace VDPackages.LocalisationPackage.Core.IO.Parsers
{
	public class CsvLocalisationParser : ILocalisationParser
	{
		private static readonly Dictionary<string, Dictionary<Language, string>> localisationEntries = new Dictionary<string, Dictionary<Language, string>>();

		public const char SEPERATION_CHARACTER = '\t';
		public const char ENTRY_COLUMN_SEPERATOR = '/';

		public bool CanPreReadAllEntries => true;

		static CsvLocalisationParser()
		{
			ReadData();
		}

		private static void ReadData()
		{
			foreach (TextAsset file in Resources.LoadAll<TextAsset>("Localisation"))
			{
				string[] lines = file.ToString().Split('\n');

				if (lines.Length < 2)
				{
					// We need at least a header and a line of localisation data to get anything useful out of a file
					continue;
				}

				if (!TryReadHeader(lines[0], out List<Language> languages, out int columnsPerLanguage))
				{
					// We couldn't read the header, so move on to the next file
					LogManager.LogError("Invalid header on " + file.name);
					continue;
				}

				for (int i = 1; i < lines.Length; i++)
				{
					string line = lines[i];
					ParseLocalisationData(line, languages, columnsPerLanguage);
				}
			}
		}

		private static bool TryReadHeader(string header, out List<Language> languages, out int columnsPerLanguage)
		{
			languages = new List<Language>();

			columnsPerLanguage = 1;
			bool hasDeterminedColumnsPerLanguage = false;
			bool hasFoundFirstLanguage = false;

			string[] languageStrings = header.Split(SEPERATION_CHARACTER);


			for (int i = 1; i < languageStrings.Length; i++) // Start at i = 1 because the first entry is just 'ID' (used as a header in the spreadsheet)
			{
				string languageString = languageStrings[i];

				if (string.IsNullOrEmpty(languageString))
				{
					if (hasFoundFirstLanguage && !hasDeterminedColumnsPerLanguage)
					{
						++columnsPerLanguage;
					}

					continue;
				}

				if (!Enum.TryParse(languageString, out Language language))
				{
					LogManager.LogError($"Failed to parse language {languageString}!");
					return false;
				}

				if (hasFoundFirstLanguage)
				{
					hasDeterminedColumnsPerLanguage = true;
				}
				else
				{
					hasFoundFirstLanguage = true;
				}

				languages.Add(language);
			}

			return true;
		}
		
		private static void ParseLocalisationData(string line, List<Language> languages, int columnsPerLanguage)
		{
			string[] values = line.Split(SEPERATION_CHARACTER);

			if (values.Length == 0 || string.IsNullOrEmpty(values[0]))
			{
				// No valid ID on this line
				return;
			}

			string id = values[0];

			int languageIndex = 0;
			int columnIndex = 0;
			
			for (int i = 1; i < values.Length; i++) // Start at 1 because the first value is the ID
			{
				string value = values[i];
				string entryID = id + ENTRY_COLUMN_SEPERATOR + columnIndex;
				
				if (!localisationEntries.TryGetValue(entryID, out Dictionary<Language, string> entryLocalisations))
				{
					entryLocalisations = new Dictionary<Language, string>();
					localisationEntries.Add(entryID, entryLocalisations);
				}
				
				if (!string.IsNullOrEmpty(value))
				{
					// Only use this value if its non-empty (for empty we can fall back to another language)
					Language language = languages[languageIndex];
					
					if (entryLocalisations.TryGetValue(language, out string localisation))
					{
						LogManager.LogWarning($"Language {language} is already defined for key {entryID}!\nCurrent value: {localisation}\nNew value: {value}");
					}
					
					entryLocalisations[language] = value;
				}

				++columnIndex;
				
				if (columnIndex == columnsPerLanguage)
				{
					columnIndex = 0;

					++languageIndex;
				}
			}
		}

		public string GetLocalisedEntry(string entryID, Language languageID)
		{
			// Not used because we can PreRead all entries, LocalisationDataManager handles this logic for us
			return LocalisationDataManager.InternalGetLocalisedEntryFromDictionary(localisationEntries, entryID, languageID);
		}

		public Dictionary<string, Dictionary<Language, string>> GetAllEntries()
		{
			return localisationEntries;
		}
	}
}