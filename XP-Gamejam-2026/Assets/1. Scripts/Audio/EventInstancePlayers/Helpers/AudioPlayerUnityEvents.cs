using System.Collections;
using AYellowpaper;
using UnityEngine;
using UnityEngine.Events;
using VDFramework;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.EventInstancePlayers.Helpers
{
	public class AudioPlayerUnityEvents : BetterMonoBehaviour
	{
		[SerializeField]
		private InterfaceReference<IAudioEventInstancePlayer> audioPlayer;
		
		public UnityEvent OnStartPlayingAudio;

		public UnityEvent OnStoppedPlayingAudio;
		
		private void Reset()
		{
			audioPlayer = new InterfaceReference<IAudioEventInstancePlayer>() { Value = GetComponent<IAudioEventInstancePlayer>() };
		}

		private void OnEnable()
		{
			audioPlayer.Value.BeforePlaying.AddCallback(OnPlayAudio);
		}

		private void OnDisable()
		{
			audioPlayer.Value.BeforePlaying.RemoveCallback(OnPlayAudio);
		}

		private void OnPlayAudio()
		{
			StartCoroutine(TrackAudioPlayingState());
		}

		private IEnumerator TrackAudioPlayingState()
		{
			OnStartPlayingAudio.Invoke();

			yield return null;

			yield return new WaitWhile(() => audioPlayer.Value.IsPlaying);
			
			OnStoppedPlayingAudio.Invoke();
        }
	}
}