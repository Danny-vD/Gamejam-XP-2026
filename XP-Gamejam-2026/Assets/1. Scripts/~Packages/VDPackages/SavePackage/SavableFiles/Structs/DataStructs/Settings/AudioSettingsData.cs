using System;
using UnityEngine;
using VDPackages.FMODUtilityPackage.Enums;
using VDPackages.SavePackage.Data.Configuration.Components;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SerializableDictionaryPackage.SerializableDictionary;

namespace VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings
{
	/// <summary>
	/// Holds settings related to audio
	/// </summary>
	[Serializable]
	public struct AudioSettingsData
	{
		public static AudioSettingsData GetDefault(AbstractSavableFile containingFile)
		{
			return new AudioSettingsData()
			{
				file         = containingFile,
				volumePerBus = new SerializableDictionary<string, float>(),
			};
		}

		[SerializeField]
		private SerializableDictionary<string, float> volumePerBus;

		private AbstractSavableFile file;

		internal void SetFile(AbstractSavableFile containingFile)
		{
			file = containingFile;
		}

		public void SetBusVolume(AudioBus bus, float volume)
		{
			volumePerBus.Add(bus.ToString(), volume);

			file.SetDirty();
		}

		public float GetBusVolume(AudioBus bus)
		{
			if (volumePerBus.TryGetValue(bus.ToString(), out float volume))
			{
				return volume;
			}

			return bus == AudioBus.Master ? DefaultSettingsHolder.Instance.DefaultMasterVolume : DefaultSettingsHolder.Instance.DefaultOtherVolume;
		}
	}
}