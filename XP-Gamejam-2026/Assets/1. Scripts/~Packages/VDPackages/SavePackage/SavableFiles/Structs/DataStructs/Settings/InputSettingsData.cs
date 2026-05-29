using System;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.SavableFiles.BaseClasses;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings
{
	/// <summary>
	/// Holds settings related to input
	/// </summary>
	[Serializable]
	public struct InputSettingsData
	{
		public static InputSettingsData GetDefault(AbstractSavableFile containingFile)
		{
			InputSettingsData defaultSettings = DefaultSettingsHolder.Instance.DefaultInputSettings;
			
			defaultSettings.file = containingFile;
			return defaultSettings;
		}

		private AbstractSavableFile file;

		public bool IsCursorConfinedToWindow;

		public void SetCursorConfinedToWindowValue(bool confined)
		{
			IsCursorConfinedToWindow = confined;
			file.SetDirty();
		}
	}
}