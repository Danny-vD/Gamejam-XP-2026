using UnityEngine;
using UnityEngine.Events;
using VDFramework;

namespace XPGJ2026.MovementSystem
{
	[DefaultExecutionOrder(1)]
	public class PlayerMovementManager : BetterMonoBehaviour
	{
		public UnityEvent OnStartTiltMovement;
		public UnityEvent OnStartWheelsMovement;
		
		[SerializeField]
		private AbstractContinuousPlayerMovement tiltMovement;
		
		[SerializeField]
		private AbstractContinuousPlayerMovement wheelsMovement;

		[Header("Settings")]
		[SerializeField]
		private float angleThreshold = 5;
		
		public bool IsUsingWheels { get; private set; }

		private void OnEnable()
		{
			IsUsingWheels = true; // Hack to ensure the switch to tilt movement happens
			SwitchToTiltMovement();
		}

		private void FixedUpdate()
		{
			float dot = Vector3.Dot(transform.up, Vector3.up);
			float angle = Vector3.Angle(Vector3.up, transform.up);

			if (dot > 0 && angle <= angleThreshold)
			{
				SwitchToWheelsMovement();
			}
			else
			{
				SwitchToTiltMovement();
			}
		}

		private void SwitchToWheelsMovement()
		{
			if (IsUsingWheels)
			{
				return;
			}
			
			IsUsingWheels = true;
			
			tiltMovement.enabled   = false;
			wheelsMovement.enabled = true;
			
			OnStartWheelsMovement.Invoke();
		}
		
		private void SwitchToTiltMovement()
		{
			if (!IsUsingWheels)
			{
				return;
			}
			
			IsUsingWheels = false;
			
			wheelsMovement.enabled = false;
			tiltMovement.enabled   = true;
			
			OnStartTiltMovement.Invoke();
		}
	}
}