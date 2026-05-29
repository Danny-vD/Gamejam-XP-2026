using AYellowpaper;
using UnityEngine;
using VDFramework;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.Objects
{
	[DefaultExecutionOrder(5)] // Delay execution order so any audio systems can run first (and parameters can be set)
	public class PlayAudioOnEnable : BetterMonoBehaviour
	{
		[Header("Audio player")]
		[SerializeField]
		private InterfaceReference<IAudioEventInstancePlayer> audioInstancePlayer;

		private void Reset()
		{
			audioInstancePlayer = new InterfaceReference<IAudioEventInstancePlayer>() { Value = GetComponent<IAudioEventInstancePlayer>() };
		}

		private void OnEnable()
		{
			audioInstancePlayer.Value.Play();
		}

		private void OnDisable()
		{
			audioInstancePlayer.Value.Stop();
		}
	}
}