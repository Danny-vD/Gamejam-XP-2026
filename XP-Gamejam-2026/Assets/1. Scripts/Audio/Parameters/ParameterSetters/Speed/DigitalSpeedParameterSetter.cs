using FMOD.Studio;
using UnityEngine;
using XPGJ2026.Audio.Parameters.ParameterSetters.BaseClasses;
using XPGJ2026.Audio.Utility;

namespace XPGJ2026.Audio.Parameters.ParameterSetters.Speed
{
	/// <summary>
	/// Sets the <see cref="AudioParameterUtil.SPEED_PARAMETER_NAME"/> parameter to 1 while moving and to 0 when not moving
	/// </summary>
	/// <remarks>
	/// After setting the value to 1, there is a cooldown period where the value will not change at all<br/>
	/// This is to prevent repeatedly starting and stopping the audio for small movements
	/// </remarks>
	public class DigitalSpeedParameterSetter : AbstractContinuousParameterSetter
	{
		[SerializeField, Tooltip("The transform that will be checked for movement (or rotation)")]
		private Transform transformToCheck;
		
		private const float waitTimerAfterMoving = 0.1f; // The cooldown before the value can be changed again, given in seconds
		private float timer = 0;

		private Vector3 oldPosition;
		private Quaternion oldOrientation;

		protected override void Reset()
		{
			base.Reset();
			transformToCheck = transform;
		}

		protected override void StartSetParameterEveryFrame(EventInstance eventInstance)
		{
			CacheCurrentTransformData();
			base.StartSetParameterEveryFrame(eventInstance);
		}

		public override void SetParameter(EventInstance eventInstance)
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;
				return;
			}
			
			if (oldOrientation != transformToCheck.rotation || oldPosition != transformToCheck.position)
			{
				eventInstance.setParameterByName(AudioParameterUtil.SPEED_PARAMETER_NAME, 1);
				timer = waitTimerAfterMoving;
			}
			else
			{
				eventInstance.setParameterByName(AudioParameterUtil.SPEED_PARAMETER_NAME, 0);
			}
			
			CacheCurrentTransformData();
		}

		private void CacheCurrentTransformData()
		{
			oldPosition    = transformToCheck.position;
			oldOrientation = transformToCheck.rotation;
		}
	}
}