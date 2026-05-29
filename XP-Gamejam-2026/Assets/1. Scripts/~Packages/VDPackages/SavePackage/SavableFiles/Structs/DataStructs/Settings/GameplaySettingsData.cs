using System;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.SavableFiles.BaseClasses;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings
{
	/// <summary>
	/// Holds settings related to general gameplay
	/// </summary>
	[Serializable]
	public struct GameplaySettingsData
	{
		public static GameplaySettingsData GetDefault(AbstractSavableFile containingFile)
		{
			GameplaySettingsData defaultSettings = DefaultSettingsHolder.Instance.DefaultGameplaySettings;
			
			defaultSettings.file = containingFile;
			return defaultSettings;
		}

		private AbstractSavableFile file;
	}
}