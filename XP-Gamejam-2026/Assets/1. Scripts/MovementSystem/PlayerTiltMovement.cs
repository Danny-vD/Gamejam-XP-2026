using System.Collections;
using EditorAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;
using VDFramework.Logger;

namespace XPGJ2026.MovementSystem
{
	public class PlayerTiltMovement : BetterMonoBehaviour
	{
		[SerializeField, HelpBox("Used to determine the direction of the input", MessageMode.None, drawAbove: true)]
		private Transform playerCameraTransform;

		[SerializeField]
		private InputActionReference movementInput;

		[SerializeField]
		private Rigidbody rigidbdy;

		[SerializeField]
		private float rotationStrength = 1;

		private Coroutine movementCoroutine;

		private void Awake()
		{
			rigidbdy = GetComponent<Rigidbody>();
		}

		private void OnEnable()
		{
			movementInput.action.performed += OnStartPressingInput;
			movementInput.action.canceled  += OnStopPressingInput;
		}

		private void OnDisable()
		{
			movementInput.action.performed -= OnStartPressingInput;
			movementInput.action.canceled  -= OnStopPressingInput;
		}

		private void OnStartPressingInput(InputAction.CallbackContext obj)
		{
			movementCoroutine = StartCoroutine(MovementHandlingCoroutine());
		}

		private void OnStopPressingInput(InputAction.CallbackContext obj)
		{
			StopCoroutine(movementCoroutine);
			movementCoroutine = null;
		}

		private IEnumerator MovementHandlingCoroutine()
		{
			while (true)
			{
				Vector2 input = movementInput.action.ReadValue<Vector2>();

				float absInputY = Mathf.Abs(input.y);
				
				if (input.y != 0)
				{
					Vector3 cameraForward = playerCameraTransform.forward;
					Vector3 forwardRotatingAxis = playerCameraTransform.right;

					Vector3 closestTransformDirection = GetClosestTransformDirection(forwardRotatingAxis);

					rigidbdy.AddTorque(input.y * rotationStrength * Time.deltaTime * closestTransformDirection, ForceMode.VelocityChange);
				}
				
				if (input.x != 0)
				{
					Vector3 cameraRight = playerCameraTransform.right;
					Vector3 rightRotatingAxis = playerCameraTransform.forward;

					Vector3 closestTranformDirection = GetClosestTransformDirection(rightRotatingAxis);

					rigidbdy.AddTorque(-input.x * rotationStrength * Time.deltaTime * closestTranformDirection, ForceMode.VelocityChange);
				}

				yield return null;
			}
		}

		private Vector3 GetClosestTransformDirection(Vector3 direction)
		{
			float dot = Vector3.Dot(direction, transform.forward);
			float biggestDotAbs = Mathf.Abs(dot);

			Vector3 closestDirection = transform.forward * Mathf.Sign(dot);
			
			dot = Vector3.Dot(direction, transform.right);
			float dotAbs = Mathf.Abs(dot);

			if (dotAbs > biggestDotAbs)
			{
				biggestDotAbs    = dotAbs;
				closestDirection = transform.right * Mathf.Sign(dot);
			}
			
			dot = Vector3.Dot(direction, transform.up);
			dotAbs = Mathf.Abs(dot);

			if (dotAbs > biggestDotAbs)
			{
				closestDirection = transform.up * Mathf.Sign(dot);
			}

			return closestDirection;
		}
	}
}