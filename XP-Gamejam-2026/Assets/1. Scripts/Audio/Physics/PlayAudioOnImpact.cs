using AYellowpaper;
using UnityEngine;
using VDFramework;
using XPGJ2026.Audio.Interfaces;
using XPGJ2026.Utility;

namespace XPGJ2026.Audio.Physics
{
	/// <summary>
	/// Plays a given <see cref="IAudioEventInstancePlayer"/> when this object hits a collider
	/// </summary>
	public class PlayAudioOnImpact : BetterMonoBehaviour
	{
		[SerializeField]
		protected ImpactDetector impactDetector;

		[Header("Audio player")]
		[SerializeField]
		protected InterfaceReference<IAudioEventInstancePlayer> audioInstancePlayer;

		protected virtual void Reset()
		{
			impactDetector      = GetComponentInParent<ImpactDetector>();
			audioInstancePlayer = new InterfaceReference<IAudioEventInstancePlayer>() { Value = GetComponent<IAudioEventInstancePlayer>() };
		}

		protected virtual void OnEnable()
		{
			impactDetector.OnImpact.AddListener(audioInstancePlayer.Value.Play);
		}

		protected virtual void OnDisable()
		{
			impactDetector.OnImpact.RemoveListener(audioInstancePlayer.Value.Play);
		}
	}
}