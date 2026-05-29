using FMOD.Studio;
using FMODUnity;
using VDFramework;
using VDFramework.ObserverPattern;
using XPGJ2026.Audio.EventInstancePlayers.Holders.Interfaces;
using XPGJ2026.Audio.Interfaces;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace XPGJ2026.Audio.EventInstancePlayers.Holders
{
	public class AudioPlayerHolder : BetterMonoBehaviour, IAudioPlayerHolder, IAudioEventInstancePlayer
	{
		public PrioritisedAction<EventInstance> BeforePlaying => AudioPlayer.BeforePlaying;
		
		public IAudioEventInstancePlayer AudioPlayer { get; private set; }
		
		public bool IsNull => AudioPlayer.IsNull;

		public bool IsPlaying => AudioPlayer.IsPlaying;
		
		public void SetAudioPlayer(IAudioEventInstancePlayer audioPlayer)
		{
			AudioPlayer = audioPlayer;
		}

		public void Play() => AudioPlayer.Play();

		public void PlayIfNotPlaying() => AudioPlayer.PlayIfNotPlaying();

		public void Stop(STOP_MODE stopMode) => AudioPlayer.Stop(stopMode);

		public void SetPause(bool paused) => AudioPlayer.SetPause(paused);

		public void SetEventReference(EventReference newEventReference) => AudioPlayer.SetEventReference(newEventReference);

		public void Cleanup(bool stopPlaying = true) => AudioPlayer.Cleanup(stopPlaying);
	}
}