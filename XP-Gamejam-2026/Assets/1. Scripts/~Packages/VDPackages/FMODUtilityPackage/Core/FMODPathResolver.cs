using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using VDFramework.Extensions;
using VDFramework.Utility;
using VDPackages.FMODUtilityPackage.Enums;
using VDPackages.FMODUtilityPackage.Structs;

namespace VDPackages.FMODUtilityPackage.Core
{
	/// <summary>
	/// Utility class that resolves <see cref="AudioBus"/>ses and <see cref="GlobalEmitter"/>s to FMOD busses and emitters 
	/// </summary>
	[Serializable]
	public class FMODPathResolver : ISerializationCallbackReceiver
	{
		public const string MASTER_BUS_PATH = "bus:/";

		private static Dictionary<AudioBus, Bus> busPerAudioBusEnum = new Dictionary<AudioBus, Bus>();

		[SerializeField]
		private List<BusPathPerBus> buses = new List<BusPathPerBus>();

		[SerializeField]
		private List<EventsPerEmitter> emitterEvents = new List<EventsPerEmitter>();

		private readonly Dictionary<GlobalEmitter, StudioEventEmitter> emitters = new Dictionary<GlobalEmitter, StudioEventEmitter>();

		public FMODPathResolver()
		{
			buses.Add(new BusPathPerBus { Key = default, Value = MASTER_BUS_PATH });
		}

		/// <summary>
		/// Will also initialize all the Dictionaries and set the Event and Bus paths
		/// </summary>
		public FMODPathResolver(bool setAllEventPaths) : this()
		{
			if (setAllEventPaths)
			{
				UpdateDictionaries();

				SetBusPaths();
			}
		}

		public void AddEmitters(GameObject gameObject)
		{
			foreach (GlobalEmitter emitterType in default(GlobalEmitter).GetValues())
			{
				StudioEventEmitter emitter = gameObject.AddComponent<StudioEventEmitter>();
				emitter.EventReference = GetEventReferenceForEmitter(emitterType);

				emitters.Add(emitterType, emitter);
			}
		}

		public Bus GetAudioBus(AudioBus audioBus)
		{
			if (!busPerAudioBusEnum.TryGetValue(audioBus, out Bus bus))
			{
				bus = GetBusFromPath(GetAudioBusPath(audioBus));
				busPerAudioBusEnum.Add(audioBus, bus);
			}

			return bus;
		}
		
		public string GetAudioBusPath(AudioBus audioBus)
		{
			return buses.First(item => item.Key.Equals(audioBus)).Value;
		}
		
		public StudioEventEmitter GetEmitter(GlobalEmitter globalEmitter)
		{
			return emitters[globalEmitter];
		}

		public static Bus GetBusFromPath(string busPath)
		{
			return RuntimeManager.GetBus(busPath);
		}
		
		private EventReference GetEventReferenceForEmitter(GlobalEmitter globalEmitter)
		{
			EventReference eventReference = emitterEvents.First(item => item.Key == globalEmitter).Value;
			return eventReference;
		}

		private void SetBusPaths()
		{
			int busCount = buses.Count;

			if (busCount <= 1) // The master bus is already taken care of in the constructor
			{
				return;
			}

			string[] busNames = default(AudioBus).GetNames().ToArray();

			// Start at 1 because 0 is always the master bus
			for (int i = 1; i < busCount; i++)
			{
				BusPathPerBus pathPerBus = buses[i];

				// Bus paths always start with bus:/ which is the Master Bus Path 
				pathPerBus.Value = MASTER_BUS_PATH + busNames[i];

				buses[i] = pathPerBus;
			}
		}

		private void UpdateDictionaries()
		{
			//TODO replace the lists with SerializableDictionaries (check the serializableDictionary drawer how to properly display it in the inspector)
			EnumDictionaryUtil.PopulateEnumDictionary<BusPathPerBus, AudioBus, string>(buses);

			EnumDictionaryUtil.PopulateEnumDictionary<EventsPerEmitter, GlobalEmitter, EventReference>(emitterEvents);
		}

		public void OnBeforeSerialize()
		{
			UpdateDictionaries();
		}

		public void OnAfterDeserialize()
		{
		}
	}
}