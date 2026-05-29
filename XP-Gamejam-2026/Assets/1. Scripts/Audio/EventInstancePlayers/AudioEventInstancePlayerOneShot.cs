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
	public class AudioEventInstancePlayerOneShot : BetterMonoBehaviour, IAudioEventInstancePlayer
	{
		public PrioritisedAction<EventInstance> BeforePlaying { get; } = new PrioritisedAction<EventInstance>();

		[SerializeField]
		private EventReference eventReference;

		private EventInstance lastInstancePlayed;

		public bool IsNull => eventReference.IsNull;
		public bool IsPlaying => AudioPlayer.IsPlaying(lastInstancePlayed);

		public void SetEventReference(EventReference newEventReference)
		{
			Cleanup(false); // Don't stop any currently playing events, generally one-shots would end by themselves and are not expected to loop

			eventReference = newEventReference;
		}

		[ContextMenu("Play")]
		public void Play()
		{
			if (!enabled) // Do not play audio if this component is disabled
			{
				return;
			}

			if (lastInstancePlayed.isValid())
			{
				lastInstancePlayed.getPaused(out bool paused);

				if (paused) // A paused instance is inaudible, and since we're a one-shot there will be no way to access it afterwards so we can stop it safely
				{
					lastInstancePlayed.stop(STOP_MODE.IMMEDIATE);
				}
				else
				{
					// If our last instance is not paused we will release it (and it will end naturally) while we grab a new one
					lastInstancePlayed.release();
					lastInstancePlayed.clearHandle();

					lastInstancePlayed = GetInstance();
				}
			}
			else
			{
				// If we have no valid instance, grab a new one
				lastInstancePlayed = GetInstance();
			}

			lastInstancePlayed.start();
			BeforePlaying.Invoke(lastInstancePlayed);
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

		public void Stop(STOP_MODE stopMode)
		{
			if (!lastInstancePlayed.isValid())
			{
				return;
			}

			lastInstancePlayed.stop(stopMode);

			lastInstancePlayed.release();
			lastInstancePlayed.clearHandle();
		}

		public void SetPause(bool paused)
		{
			if (!lastInstancePlayed.isValid())
			{
				return;
			}

			lastInstancePlayed.setPaused(paused);
		}

		private EventInstance GetInstance()
		{
			return AudioPlayer.GetEventInstance(eventReference);
		}

		public void Cleanup(bool stopPlaying = true)
		{
			if (stopPlaying)
			{
				Stop(STOP_MODE.IMMEDIATE);
			}

			if (lastInstancePlayed.isValid())
			{
				lastInstancePlayed.release();
				lastInstancePlayed.clearHandle();
			}
		}
	}
}