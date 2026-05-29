using UnityEngine;
using VDFramework.Extensions;
using VDPackages.FMODUtilityPackage.Core;
using VDPackages.FMODUtilityPackage.Enums;
using VDPackages.LocalisationPackage.Core.Enums;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.Data.Constants;
using VDPackages.SavePackage.SavableFiles.SpecificClasses;

namespace VDPackages.SavePackage.Data.Configuration.Static
{
	public static class SettingsReader
	{
		public static void ApplyAllSettings(GenericConfigFile configFile)
		{
			ApplyDisplaySettings(configFile);
			ApplyInputSettings(configFile);
			ApplyAudioSettings(configFile);
			ApplyLanguageSettings(configFile);
			ApplyGameplaySettings(configFile);
		}
		
		public static void ApplyDisplaySettings(GenericConfigFile configFile)
		{
			Application.targetFrameRate = configFile.DisplaySettingsData.TargetFrameRate;
			QualitySettings.vSyncCount  = configFile.DisplaySettingsData.VSyncCount;
		}
		
		public static void ApplyInputSettings(GenericConfigFile configFile)
		{
			Cursor.lockState = configFile.InputSettingsData.IsCursorConfinedToWindow ? CursorLockMode.Confined : CursorLockMode.None;
		}

		public static void ApplyAudioSettings(GenericConfigFile configFile)
		{
			foreach (AudioBus audioBus in default(AudioBus).GetValues())
			{
				float volume = configFile.AudioSettingsData.GetBusVolume(audioBus);
				
				AudioVolumeManager.SetBusVolume(audioBus, volume);
			}
		}

		public static void ApplyLanguageSettings(GenericConfigFile configFile)
		{
			configFile.LanguageSettingsData.TryGetLanguageValue(ConfigurationConstants.CURRENT_LANGUAGE_KEY, out Language language, DefaultSettingsHolder.GetDefaultLanguage);
			
			LocalisationPackage.Core.LanguageSettings.Language = language;
		}
				
		public static void ApplyGameplaySettings(GenericConfigFile configFile)
		{
		}
	}
}