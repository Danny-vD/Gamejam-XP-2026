using VDFramework;
using VDPackages.SavePackage.Data.Configuration.Static;
using VDPackages.SavePackage.SavableFiles.SpecificClasses;
using VDPackages.SavePackage.Utility;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.AudioSettings
{
	public class WriteAudioSettingsComponent : BetterMonoBehaviour
	{
		private void OnDisable()
		{
			WriteSettingsToCurrent();
		}

		private static void WriteSettingsToCurrent()
		{
			GenericConfigFile configFile = CurrentFileHelper.GetCurrentFile<GenericConfigFile>();
            
			SettingsWriter.WriteAudioSettings(configFile);
		}
	}
}