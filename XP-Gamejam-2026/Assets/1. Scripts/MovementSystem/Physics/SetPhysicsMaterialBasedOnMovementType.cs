using EditorAttributes;
using UnityEngine;
using VDFramework;

namespace XPGJ2026.MovementSystem.Physics
{
	public class SetPhysicsMaterialBasedOnMovementType : BetterMonoBehaviour
	{
		[SerializeField, Required(fixMode: ReferenceFixMode.Scene)]
		private PlayerMovementManager playerMovementManager;

		[SerializeField, Required(fixMode: ReferenceFixMode.Self)]
		private Collider collidr;

		[SerializeField]
		private PhysicsMaterial tiltMovementMaterial;
		
		[SerializeField]
		private PhysicsMaterial wheelsMovementMaterial;

		private void OnEnable()
		{
			playerMovementManager.OnStartTiltMovement.AddListener(OnStartTiltMovement);
			playerMovementManager.OnStartWheelsMovement.AddListener(OnStartWheelsMovement);
		}

		private void OnDisable()
		{
			playerMovementManager.OnStartTiltMovement.RemoveListener(OnStartTiltMovement);
			playerMovementManager.OnStartWheelsMovement.RemoveListener(OnStartWheelsMovement);
		}

		private void OnStartTiltMovement()
		{
			collidr.material = tiltMovementMaterial;
		}

		private void OnStartWheelsMovement()
		{
			collidr.material = wheelsMovementMaterial;
		}
	}
}