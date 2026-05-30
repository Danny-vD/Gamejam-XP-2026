using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

namespace XPGJ2026.MovementSystem
{
	public abstract class AbstractContinuousPlayerMovement : BetterMonoBehaviour
	{
		[SerializeField]
		protected InputActionReference movementInput;

		private Coroutine movementCoroutine;
		
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
			movementCoroutine ??= StartCoroutine(MovementHandlingCoroutine());
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
				HandleInput();

				yield return null;
			}
		}

		protected abstract void HandleInput();
		
		protected static Vector3 GetClosestDirection(Vector3 target, Vector3[] directions)
		{
			float biggestAbsDot = 0;
			Vector3 closestDirection = Vector3.zero;

			foreach (Vector3 direction in directions)
			{
				float dot = Vector3.Dot(direction, target);
				float absDot = Mathf.Abs(dot);

				if (absDot > biggestAbsDot)
				{
					biggestAbsDot    = absDot;
					closestDirection = direction * Mathf.Sign(dot);
				}
			}

			return closestDirection;
		}
	}
}