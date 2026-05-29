using EditorAttributes;
using UnityEngine;
using VDFramework.Singleton;
using VDPackages.LocalisationPackage.Core;
using VDPackages.LocalisationPackage.Core.Enums;
using VDPackages.SavePackage.SavableFiles.Structs.DataStructs;
using VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings;

namespace VDPackages.SavePackage.Data.Configuration.Components
{
	public class DefaultSettingsHolder : Singleton<DefaultSettingsHolder>
	{
		public DisplaySettingsData DefaultDisplaySettings;

		[Line(GUIColor.Gray)]
		[Space]
		public InputSettingsData DefaultInputSettings;
		
		[Line(GUIColor.Gray)]
		[Space]
		public float DefaultMasterVolume = 0.8f;
		public float DefaultOtherVolume = 1;
		
		[Line(GUIColor.Gray)]
		[Space]
		public GameplaySettingsData DefaultGameplaySettings;
		
		[Line(GUIColor.Gray)]
		[Space]
		public GameData DefaultGameSettings;

		public static Language GetDefaultLanguage()
		{
			return LanguageSettings.GetDefaultLanguage();
		}
	}
}