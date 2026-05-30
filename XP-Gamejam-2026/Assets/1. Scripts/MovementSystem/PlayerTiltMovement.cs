using System.Collections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

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

				if (input.y != 0)
				{
					Vector3 cameraForward = playerCameraTransform.forward;
					Vector3 forwardRotatingAxis = Vector3.Cross(Vector3.up, cameraForward).normalized;

					rigidbdy.AddTorque(forwardRotatingAxis * input.y);
				}

				if (input.x != 0)
				{
					Vector3 cameraRight = playerCameraTransform.right;
					Vector3 rightRotatingAxis = Vector3.Cross(cameraRight, Vector3.down).normalized;
					
					rigidbdy.AddTorque(rightRotatingAxis * input.x);
				}

				yield return null;
			}
		}
	}
}