using System.Collections;
using UnityEngine;
using VDFramework;
using XPGJ2026.Audio.Utility;

namespace XPGJ2026.Audio.Volume
{
	public class SetLevelStartParameterOnEnable : BetterMonoBehaviour
	{
		[SerializeField]
		private float unmuteDelay;

		private void OnEnable()
		{
			AudioParameterUtil.SetLevelStartGlobalParameter(true);

			StartCoroutine(UnmuteAfterDelay());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		public IEnumerator UnmuteAfterDelay()
		{
			yield return new WaitForSeconds(unmuteDelay);

			AudioParameterUtil.SetLevelStartGlobalParameter(false);
		}
	}
}