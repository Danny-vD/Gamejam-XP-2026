using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using VDFramework;

namespace XPGJ2026.MovementSystem
{
	public abstract class AbstractContinuousPlayerMovement : BetterMonoBehaviour
	{
		public UnityEvent onMoving;
		public UnityEvent onStopMoving;

		[SerializeField]
		protected InputActionReference movementInput;

		private Coroutine movementCoroutine;

		protected virtual void OnEnable()
		{
			movementInput.action.performed += OnStartPressingInput;
			movementInput.action.canceled  += OnStopPressingInput;
		}

		protected virtual void OnDisable()
		{
			movementInput.action.performed -= OnStartPressingInput;
			movementInput.action.canceled  -= OnStopPressingInput;
		}

		private void OnStartPressingInput(InputAction.CallbackContext obj)
		{
			movementCoroutine ??= StartCoroutine(MovementHandlingCoroutine());
			onMoving.Invoke();
		}

		private void OnStopPressingInput(InputAction.CallbackContext obj)
		{
			if (movementCoroutine != null)
			{
				StopCoroutine(movementCoroutine);
			}

			movementCoroutine = null;
			onStopMoving.Invoke();
		}

		private IEnumerator MovementHandlingCoroutine()
		{
			while (true)
			{
				HandleInput();

				yield return GetYieldInstruction();
			}
		}

		protected abstract void HandleInput();

		protected abstract YieldInstruction GetYieldInstruction();

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