using UnityEngine;
using VDFramework.Extensions;
using VDPackages.FMODUtilityPackage.Core;
using VDPackages.FMODUtilityPackage.Enums;
using VDPackages.SavePackage.Data.Constants;
using VDPackages.SavePackage.SavableFiles.SpecificClasses;

namespace VDPackages.SavePackage.Data.Configuration.Static
{
	public static class SettingsWriter
	{
		public static void WriteAllSettings(GenericConfigFile configFile)
		{
			WriteDisplaySettings(configFile);
			WriteInputSettings(configFile);
			WriteAudioSettings(configFile);
			WriteLanguageSettings(configFile);
			WriteGameplaySettings(configFile);
		}
		
		public static void WriteDisplaySettings(GenericConfigFile configFile)
		{
			configFile.DisplaySettingsData.SetTargetFrameRate(Application.targetFrameRate);
			configFile.DisplaySettingsData.SetVSyncCount(QualitySettings.vSyncCount);
		}
		
		public static void WriteInputSettings(GenericConfigFile configFile)
		{
			configFile.InputSettingsData.SetCursorConfinedToWindowValue(Cursor.lockState == CursorLockMode.Confined);
		}
		
		public static void WriteAudioSettings(GenericConfigFile configFile)
		{
			foreach (AudioBus audioBus in default(AudioBus).GetValues())
			{
				float volume = AudioVolumeManager.GetBusVolume(audioBus);

				configFile.AudioSettingsData.SetBusVolume(audioBus, volume);
			}
		}
		
		public static void WriteLanguageSettings(GenericConfigFile configFile)
		{
			configFile.LanguageSettingsData.SetLanguageValue(ConfigurationConstants.CURRENT_LANGUAGE_KEY, VDPackages.LocalisationPackage.Core.LanguageSettings.Language);
		}
		
		public static void WriteGameplaySettings(GenericConfigFile configFile)
		{
		}
	}
}