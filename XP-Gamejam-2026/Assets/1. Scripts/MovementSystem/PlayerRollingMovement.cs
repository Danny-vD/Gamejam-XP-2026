using EditorAttributes;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

namespace XPGJ2026.MovementSystem
{
	public class PlayerRollingMovement : AbstractContinuousPlayerMovement
	{

		[field: Header("Ambience")]

		[field: SerializeField] public EventReference ambience { get; private set; }

		[SerializeField, HelpBox("Used to determine the direction of the input", MessageMode.None, drawAbove: true)]
		private Transform playerCameraTransform;

		[SerializeField]
		private Rigidbody rigidbdy;

		[FormerlySerializedAs("forcePointLeftWheel")]
		[Space]
		[SerializeField, DrawHandle(handleSpace: Space.Self)]
		private Vector3 forcePointLeftWheelLocal;

		[FormerlySerializedAs("forcePointRightWheel")]
		[SerializeField, DrawHandle(handleSpace: Space.Self)]
		private Vector3 forcePointRightWheelLocal;

		[SerializeField]
		private Vector3 forwardForceDirectionLocal = Vector3.right;

		[SerializeField]
		private float maxStrengthModifier = 2;
		
		[Header("Speed")]
		[SerializeField]
		private float rollingAcceleration = 3;
		
		[SerializeField]
		private float rotateSpeed = 3;

		[Header("Max Velocity")]
		[SerializeField, Tooltip("The maximum angular velocity in degrees per second")]
		private float maxAngularVelocityDegrees = 45;
		
		[SerializeField, Tooltip("The maximum linear velocity in metres per second")]
		private float maxLinearVelocity = 2;

		private readonly YieldInstruction yieldInstruction = new WaitForFixedUpdate();

		[Button("Set max velocity")]
		private void SetMaxVelocity()
		{
			rigidbdy.maxAngularVelocity = maxAngularVelocityDegrees * Mathf.Deg2Rad;
			rigidbdy.maxLinearVelocity  = maxLinearVelocity;
		}

		private void Reset()
		{
			playerCameraTransform = FindAnyObjectByType<Camera>().transform;

			rigidbdy = GetComponent<Rigidbody>();
		}

		private void Awake()
		{
			forwardForceDirectionLocal.Normalize();

			rigidbdy.maxAngularVelocity = maxAngularVelocityDegrees * Mathf.Deg2Rad;
			rigidbdy.maxLinearVelocity  = maxLinearVelocity;
		}

		protected override void HandleInput()
		{
			Vector2 input = movementInput.action.ReadValue<Vector2>();

			if (input.y == 0)
			{
				return;
			}

			Vector3 forceDirectionWorld = transform.TransformDirection(forwardForceDirectionLocal);
			
			float dot = Vector3.Dot(playerCameraTransform.forward, forceDirectionWorld);

			if (dot < 0)
			{
				forceDirectionWorld *= -1;
			}

			float directionalWeaknessModifierLeft = Mathf.InverseLerp(-1, 0, input.x);
			float directionalWeaknessModifierRight = Mathf.InverseLerp(1, 0, input.x);
			
			float directionalStrengthModifierLeft = Mathf.Lerp(1, maxStrengthModifier, 1 - directionalWeaknessModifierRight);
			float directionalStrengthModifierRight = Mathf.Lerp(1, maxStrengthModifier, 1 - directionalWeaknessModifierLeft);
			
			Vector3 forceLeftWorld = input.y * rollingAcceleration * directionalWeaknessModifierLeft * directionalStrengthModifierLeft * forceDirectionWorld;
			Vector3 forceRightWorld = input.y * rollingAcceleration * directionalWeaknessModifierRight * directionalStrengthModifierRight * forceDirectionWorld;

			Vector3 leftWheelPositionWorld = transform.TransformPoint(forcePointLeftWheelLocal);
			Vector3 rightWheelPositionWorld = transform.TransformPoint(forcePointRightWheelLocal);

			rigidbdy.AddForceAtPosition(forceLeftWorld, leftWheelPositionWorld, ForceMode.Acceleration);
			rigidbdy.AddForceAtPosition(forceRightWorld, rightWheelPositionWorld, ForceMode.Acceleration);

			if (input.x != 0)
			{
				rigidbdy.AddTorque(input.x * rotateSpeed * transform.up, ForceMode.Impulse);
			}
		}

		protected override YieldInstruction GetYieldInstruction()
		{
			return yieldInstruction;
		}
	}
}