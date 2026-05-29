using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using VDFramework;
using VDFramework.ObserverPattern;
using VDPackages.FMODUtilityPackage.Core;
using XPGJ2026.Audio.Interfaces;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace XPGJ2026.Audio.EventInstancePlayers
{
	public class AudioEventInstancePlayer : BetterMonoBehaviour, IAudioEventInstancePlayer
	{
		public PrioritisedAction<EventInstance> BeforePlaying { get; } = new PrioritisedAction<EventInstance>();

		[SerializeField]
		private EventReference eventReference;

		/// <summary>
		/// Tests if the EventReference is null
		/// </summary>
		public bool IsNull => eventReference.IsNull;

		public bool IsPlaying => AudioPlayer.IsPlaying(GetInstance());
		
		private EventInstance eventInstance = new EventInstance(IntPtr.Zero);
		
		public void SetEventReference(EventReference newEventReference)
		{
			Cleanup(true); // Stop the current playing event, there's no guarantee it would end naturally (it might be looping)

			eventReference = newEventReference;
		}

		[ContextMenu("Play")]
		public void Play()
		{
			if (!enabled) // Do not play audio if this component is disabled
			{
				return;
			}
			
			EventInstance instance = GetInstance();

			BeforePlaying.Invoke(instance);
			instance.start();
		}

		[ContextMenu("PlayIfNotPlaying")]
		public void PlayIfNotPlaying()
		{
			if (!IsPlaying)
			{
				Play();
			}
		}

		[ContextMenu("Stop")]
		public void Stop()
		{
			Stop(STOP_MODE.ALLOWFADEOUT);
		}

		public void SetPause(bool paused)
		{
			GetInstance().setPaused(paused);
		}

		public void Stop(STOP_MODE stopMode)
		{
			GetInstance().stop(stopMode);
		}

		public EventInstance GetInstance()
		{
			if (!eventInstance.isValid())
			{
				eventInstance = AudioPlayer.GetEventInstance(eventReference);
			}

			return eventInstance;
		}

		public void Cleanup(bool stopPlaying = true)
		{
			if (!eventInstance.isValid())
			{
				return;
			}
			
			if (stopPlaying)
			{
				eventInstance.stop(STOP_MODE.IMMEDIATE);
			}

			eventInstance.release();
			eventInstance.clearHandle();
		}

		private void OnDestroy()
		{
			Cleanup();
		}
	}
}