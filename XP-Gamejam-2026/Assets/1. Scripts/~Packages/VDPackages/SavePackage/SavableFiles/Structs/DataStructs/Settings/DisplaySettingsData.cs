using System;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.SavableFiles.BaseClasses;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings
{
	/// <summary>
	/// Holds settings related to general gameplay
	/// </summary>
	[Serializable]
	public struct DisplaySettingsData
	{
		public static DisplaySettingsData GetDefault(AbstractSavableFile containingFile)
		{
			DisplaySettingsData defaultSettings = DefaultSettingsHolder.Instance.DefaultDisplaySettings;
			
			defaultSettings.file = containingFile;
			return defaultSettings;
		}

		private AbstractSavableFile file;

		public int TargetFrameRate;
		public int VSyncCount;

		public void SetTargetFrameRate(int fps)
		{
			TargetFrameRate = fps;
			file.SetDirty();
		}

		public void SetVSyncCount(int vsyncCount)
		{
			VSyncCount = vsyncCount;
			file.SetDirty();
		}
	}
}