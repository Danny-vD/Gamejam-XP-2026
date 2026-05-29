using System;
using UnityEngine;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.SpecificClasses;
using VDPackages.SerializableDictionaryPackage.SerializableDictionary;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs
{
	/// <summary>
	/// Holds data specific to the game
	/// </summary>
	[Serializable]
	public struct GameData
	{
		public static GameData GetDefault(AbstractSavableFile containingFile)
		{
			if (containingFile is GenericConfigFile)
			{
				GameData defaultSettings = DefaultSettingsHolder.Instance.DefaultGameSettings;
			
				defaultSettings.file = containingFile;
				return defaultSettings;
			}
			
			return new GameData()
			{
				file              = containingFile,
				enumValuesPerID   = new SerializableDictionary<string, string>(),
				floatValuesPerID  = new SerializableDictionary<string, float>(),
				intValuesPerID    = new SerializableDictionary<string, int>(),
				stringValuesPerID = new SerializableDictionary<string, string>(),
				boolValuesPerID = new SerializableDictionary<string, bool>(),
			};
		}

		[SerializeField]
		private SerializableDictionary<string, string> enumValuesPerID;

		[SerializeField]
		private SerializableDictionary<string, float> floatValuesPerID;

		[SerializeField]
		private SerializableDictionary<string, int> intValuesPerID;

		[SerializeField]
		private SerializableDictionary<string, string> stringValuesPerID;
		
		[SerializeField]
		private SerializableDictionary<string, bool> boolValuesPerID;

		private AbstractSavableFile file;

		public void SetEnumValue<TEnum>(string id, TEnum value) where TEnum : Enum
		{
			enumValuesPerID.Add(id, value.ToString());

			file.SetDirty();
		}

		public bool TryGetEnumValue<TEnum>(string id, out TEnum value, TEnum defaultValue = default) where TEnum : Enum
		{
			if (enumValuesPerID.TryGetValue(id, out string enumString))
			{
				if (Enum.TryParse(typeof(TEnum), enumString, false, out object result))
				{
					value = (TEnum)result;
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

		public void SetFloatValue(string id, float value)
		{
			floatValuesPerID.Add(id, value);

			file.SetDirty();
		}

		public bool TryGetFloatValue(string id, out float value)
		{
			return floatValuesPerID.TryGetValue(id, out value);
		}

		public void SetIntValue(string id, int value)
		{
			intValuesPerID.Add(id, value);

			file.SetDirty();
		}

		public bool TryGetIntValue(string id, out int value)
		{
			return intValuesPerID.TryGetValue(id, out value);
		}

		public void SetStringValue(string id, string value)
		{
			stringValuesPerID.Add(id, value);

			file.SetDirty();
		}

		public bool TryGetStringValue(string id, out string value)
		{
			return stringValuesPerID.TryGetValue(id, out value);
		}
		
		public void SetBoolValue(string id, bool value)
		{
			boolValuesPerID.Add(id, value);

			file.SetDirty();
		}

		public bool TryGetBoolValue(string id, out bool value)
		{
			return boolValuesPerID.TryGetValue(id, out value);
		}
	}
}