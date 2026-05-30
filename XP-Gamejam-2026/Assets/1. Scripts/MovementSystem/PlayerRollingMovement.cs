using EditorAttributes;
using UnityEngine;

namespace XPGJ2026.MovementSystem
{
	public class PlayerRollingMovement : AbstractContinuousPlayerMovement
	{
		[SerializeField, HelpBox("Used to determine the direction of the input", MessageMode.None, drawAbove: true)]
		private Transform playerCameraTransform;
		
		[SerializeField]
		private Rigidbody rigidbdy;

		[Space]
		[SerializeField, DrawHandle(handleSpace: Space.Self)]
		private Vector3 forcePointLeftWheel;
		
		[SerializeField, DrawHandle(handleSpace: Space.Self)]
		private Vector3 forcePointRightWheel;

		[SerializeField]
		private float rollingStrength = 1;

		[SerializeField]
		private Vector3 forwardForceDirection = Vector3.right;
		
		private void Reset()
		{
			playerCameraTransform = FindAnyObjectByType<Camera>().transform;

			rigidbdy = GetComponent<Rigidbody>();
		}

		private void Awake()
		{
			forwardForceDirection.Normalize();
		}

		protected override void HandleInput()
		{
			Vector2 input = movementInput.action.ReadValue<Vector2>();

			float dot = Vector3.Dot(playerCameraTransform.forward, forwardForceDirection);

			Vector3 forceDirection = transform.InverseTransformDirection(forwardForceDirection);

			if (dot < 0)
			{
				forceDirection *= -1;
			}

			Vector3 forceLeft = input.y * rollingStrength * forceDirection;
			Vector3 forceRight = input.y * rollingStrength * forceDirection;
			
			rigidbdy.AddForceAtPosition(forceLeft, forcePointLeftWheel);
			rigidbdy.AddForceAtPosition(forceRight, forcePointRightWheel);
		}
	}
}