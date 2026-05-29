using UnityEngine;
using VDFramework;
using XPGJ2026.Audio.Utility;

namespace XPGJ2026.Audio.Parameters
{
	[DisallowMultipleComponent]
	public class GenericAudioParameterValues : BetterMonoBehaviour
	{
		protected Camera maincamera;

		protected virtual void Awake()
		{
			maincamera = Camera.main;
		}

		public float GetPanValue()
		{
			if (maincamera == null)
			{
				maincamera = Camera.main;

				if (maincamera == null)
				{
					return 0;
				}
			}

			return AudioParameterUtil.GetPanParameterValue(transform.position, maincamera);
		}
	}
}