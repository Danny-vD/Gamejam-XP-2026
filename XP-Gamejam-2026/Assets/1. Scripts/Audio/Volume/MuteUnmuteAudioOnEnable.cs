using System.Collections;
using UnityEngine;
using VDFramework;
using VDPackages.FMODUtilityPackage.Core;
using VDPackages.FMODUtilityPackage.Enums;

namespace XPGJ2026.Audio.Volume
{
	public class MuteUnmuteAudioOnEnable : BetterMonoBehaviour
	{
		[SerializeField]
		private AudioBus audioBus;

		[SerializeField]
		private bool unmuteAfterDelay;

		[SerializeField]
		private float unmuteDelay;
		
		private void OnEnable()
		{
			AudioVolumeManager.SetBusMute(audioBus, true);

			if (unmuteAfterDelay)
			{
				StartCoroutine(UnmuteAfterDelay());
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		public IEnumerator UnmuteAfterDelay()
		{
			yield return new WaitForSeconds(unmuteDelay);
			
			AudioVolumeManager.SetBusMute(audioBus, false);
		}
	}
}