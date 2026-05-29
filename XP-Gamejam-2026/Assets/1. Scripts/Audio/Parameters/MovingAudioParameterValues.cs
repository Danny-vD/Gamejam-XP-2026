using EditorAttributes;
using UnityEngine;

namespace XPGJ2026.Audio.Parameters
{
	[DefaultExecutionOrder(100)]
	public class MovingAudioParameterValues : GenericAudioParameterValues
	{
		[SerializeField, Tooltip("The speed that will return normalizedSpeed = 1")]
		private float maxSpeed = 25;

		[SerializeField]
		private bool accountForRotation = true;

		[ShowField(nameof(accountForRotation))]
		[SerializeField, Tooltip("Set this to a child transform that is as far away as possible from this transform while still on the object")]
		private Transform furthestPointFromCentre;

		private Vector3 previousPosition;

		private bool calculatedSpeedThisFrame = false;

		private Vector3 totalVelocity;
		private float speed;

		private Rigidbody2D rigidbdy;

		protected override void Awake()
		{
			base.Awake();
			rigidbdy = GetComponentInParent<Rigidbody2D>();
		}

		private void LateUpdate()
		{
			previousPosition = transform.position;

			calculatedSpeedThisFrame = false;
		}

		public float GetNormalizedSpeedValue()
		{
			if (!calculatedSpeedThisFrame)
			{
				CalculateSpeed();
			}

			return speed / maxSpeed;
		}

		public float GetSpeedValue(out Vector2 velocity)
		{
			if (!calculatedSpeedThisFrame)
			{
				CalculateSpeed();
			}

			velocity = totalVelocity;
			return speed;
		}

		private void CalculateSpeed()
		{
			calculatedSpeedThisFrame = true;

			Vector3 tangentialVelocity = Vector2.zero; // The velocity that comes from the angular velocity
			float angularSpeedPerSecond = 0;

			if (accountForRotation && !ReferenceEquals(furthestPointFromCentre, null) && rigidbdy != null)
			{
				// Rigidbody dynamics calculations to combine linear and angular velocity
				float angularVelocityRadians = rigidbdy.angularVelocity * Mathf.Deg2Rad;
				Vector3 angularVelocityVector = new Vector3(0, 0, angularVelocityRadians);

				Vector3 radiusVector = furthestPointFromCentre.position - transform.position;

				tangentialVelocity = Vector3.Cross(angularVelocityVector, radiusVector);

				angularSpeedPerSecond = Mathf.Abs(angularVelocityRadians) * radiusVector.magnitude;
			}

			Vector2 linearVelocity = transform.position - previousPosition;

			speed =  linearVelocity.magnitude / Time.deltaTime;
			speed += angularSpeedPerSecond;

			linearVelocity     /= Time.deltaTime;
			tangentialVelocity /= Time.deltaTime;

			totalVelocity = (Vector3)linearVelocity + tangentialVelocity;
		}
	}
}