using System;
using EditorAttributes;
using UnityEngine;

namespace XPGJ2026.MovementSystem
{
	public class PlayerTiltMovement : AbstractContinuousPlayerMovement
	{
		[SerializeField, HelpBox("Used to determine the direction of the input", MessageMode.None, drawAbove: true)]
		private Transform playerCameraTransform;

		[SerializeField]
		private Rigidbody rigidbdy;

		[SerializeField]
		private float rotationStrength = 1;

		private void Reset()
		{
			playerCameraTransform = FindAnyObjectByType<Camera>().transform;

			rigidbdy = GetComponent<Rigidbody>();
		}

		protected override void HandleInput()
		{
			Vector2 input = movementInput.action.ReadValue<Vector2>();

			Vector3[] allowedRotatingAxii = GetAllowedRotatingAxii();

			float absInputX = Mathf.Abs(input.x);
			float absInputY = Mathf.Abs(input.y);

			if (input.y != 0 && absInputY >= absInputX)
			{
				Vector3 cameraForward = playerCameraTransform.forward;
				Vector3 forwardRotatingAxis = playerCameraTransform.right;

				Vector3 closestTransformDirection = GetClosestDirection(forwardRotatingAxis, allowedRotatingAxii);

				rigidbdy.AddTorque(input.y * rotationStrength * Time.deltaTime * closestTransformDirection, ForceMode.VelocityChange);
			}

			if (input.x != 0 && absInputX > absInputY)
			{
				Vector3 cameraRight = playerCameraTransform.right;
				Vector3 rightRotatingAxis = playerCameraTransform.forward;

				Vector3 closestTranformDirection = GetClosestDirection(rightRotatingAxis, allowedRotatingAxii);

				rigidbdy.AddTorque(-input.x * rotationStrength * Time.deltaTime * closestTranformDirection, ForceMode.VelocityChange);
			}
		}

		/// <summary>
		/// Returns 2 out of 3 of the base transform directions, with the one closest to the surface normal being discarded
		/// </summary>
		/// <returns></returns>
		private Vector3[] GetAllowedRotatingAxii()
		{
			Vector3 surfaceNormal = Vector3.up; //TODO: Get actual surfaceNormal

			Tuple<Vector3, float>[] dotPerTransformDirection = new Tuple<Vector3, float>[3];

			// Go over every direction (forward, right, up) to find the one which goes the most into the surface normal direction
			float dot = Vector3.Dot(transform.forward, surfaceNormal);

			float biggestAbsDot = Mathf.Abs(dot);
			int biggestAbsDotIndex = 0;

			dotPerTransformDirection[0] = new Tuple<Vector3, float>(transform.forward, dot);

			dot = Vector3.Dot(transform.right, surfaceNormal);
			float absDot = Mathf.Abs(dot);

			if (absDot > biggestAbsDot)
			{
				biggestAbsDot      = absDot;
				biggestAbsDotIndex = 1;
			}

			dotPerTransformDirection[1] = new Tuple<Vector3, float>(transform.right, dot);

			dot    = Vector3.Dot(transform.up, surfaceNormal);
			absDot = Mathf.Abs(dot);

			if (absDot > biggestAbsDot)
			{
				biggestAbsDotIndex = 2;
			}

			dotPerTransformDirection[2] = new Tuple<Vector3, float>(transform.up, dot);

			Vector3[] result = new Vector3[2];
			int resultIndex = 0;

			for (int i = 0; i < dotPerTransformDirection.Length; i++)
			{
				if (i == biggestAbsDotIndex)
				{
					continue;
				}

				result[resultIndex] = dotPerTransformDirection[i].Item1;
				++resultIndex;
			}

			return result;
		}
	}
}