using System;
using UnityEngine;
using VDPackages.LocalisationPackage.Core;
using VDPackages.LocalisationPackage.Core.Enums;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SerializableDictionaryPackage.SerializableDictionary;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings
{
	/// <summary>
	/// Holds settings related to language
	/// </summary>
	[Serializable]
	public struct LanguageSettingsData
	{
		public static LanguageSettingsData GetDefault(AbstractSavableFile containingFile)
		{
			return new LanguageSettingsData()
			{
				file           = containingFile,
				languagesPerID = new SerializableDictionary<string, string>(),
			};
		}

		[SerializeField]
		private SerializableDictionary<string, string> languagesPerID;

		private AbstractSavableFile file;

		public void SetLanguageValue(string id, Language value)
		{
			languagesPerID.Add(id, value.ToString());

			file.SetDirty();
		}

		public bool TryGetLanguageValue(string id, out Language value, Language defaultValue = LanguageSettings.DEFAULT_LANGUAGE)
		{
			if (languagesPerID.TryGetValue(id, out string languageString))
			{
				if (Enum.TryParse(typeof(Language), languageString, false, out object result))
				{
					value = (Language)result;
				}
				else
				{
					value = defaultValue;
				}

				return true;
			}

			value = defaultValue;
			return false;
		}
		
		public bool TryGetLanguageValue(string id, out Language value, Func<Language> defaultValue)
		{
			if (languagesPerID.TryGetValue(id, out string languageString))
			{
				if (Enum.TryParse(typeof(Language), languageString, false, out object result))
				{
					value = (Language)result;
				}
				else
				{
					value = defaultValue.Invoke();
				}

				return true;
			}

			value = defaultValue.Invoke();
			return false;
		}
	}
}