using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

namespace XPGJ2026.CameraSystem
{
	public class ThirdPersonCameraController : BetterMonoBehaviour
	{
		[SerializeField]
		private InputActionReference mouseDelta;

		[SerializeField, Tooltip("The transform that will be followed by the camera")]
		private Transform target;

		[SerializeField, Tooltip("The maximum distance the camera will be from the target")]
		private float maximumDistance;

		[SerializeField, Tooltip("The distance to stay away from the raycasted distance in case the maximum is not reached")]
		private float distancePadding = 1;

		[SerializeField]
		private Vector3 offset = Vector3.zero;

		[SerializeField]
		private Vector2 rotationSpeed = Vector2.one;

		[SerializeField, Tooltip("The minimum and maximum amount that can be rotated around the camera local X-axis")]
		private Vector2 minMaxYRotation = new Vector2(0, 90);

		[SerializeField, Tooltip("Layers that will be allowed to block the camera line of sight to the target")]
		private LayerMask ignoreLayers = 0;

		private float totalRotatedY;

		private void LateUpdate()
		{
			Vector3 targetPosition = target.position;

			CachedTransform.position = targetPosition;

			Vector2 delta = mouseDelta.action.ReadValue<Vector2>();

			if (delta.sqrMagnitude > 0)
			{
				RotateAroundTarget(delta);
			}

			Vector3 currentPosition = CachedTransform.position;
			Vector3 direction = currentPosition - CachedTransform.forward * maximumDistance + offset - currentPosition;
			
			if (Physics.Raycast(currentPosition, direction, out RaycastHit hitinfo, maximumDistance, ~ignoreLayers))
			{
				CachedTransform.position = hitinfo.point + CachedTransform.forward * distancePadding;
			}
			else
			{
				CachedTransform.position -= CachedTransform.forward * maximumDistance + offset;
			}
		}

		private void RotateAroundTarget(Vector2 delta)
		{
			if (delta.x != 0)
			{
				float xRotation = delta.x * rotationSpeed.x;

				CachedTransform.Rotate(Vector3.up, xRotation, Space.World);
			}

			if (delta.y != 0)
			{
				// mouse Y is reversed
				float yRotation = -delta.y * rotationSpeed.y;

				float newTotalRotation = totalRotatedY + yRotation;

				if (newTotalRotation < minMaxYRotation.x || newTotalRotation > minMaxYRotation.y)
				{
					return;
				}

				CachedTransform.Rotate(Vector3.right, yRotation);
				totalRotatedY = newTotalRotation;
			}
		}
	}
}