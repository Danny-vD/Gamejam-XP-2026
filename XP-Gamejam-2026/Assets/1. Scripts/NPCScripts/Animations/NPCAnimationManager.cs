using EditorAttributes;
using UnityEngine;
using VDFramework;
using XPGJ2026.PlayerCollisionHandlers.Interfaces;

namespace XPGJ2026.NPCScripts.Animations
{
	public class NPCAnimationManager : BetterMonoBehaviour, IPlayerCollisionHandler
	{
		[SerializeField, Required(fixMode: ReferenceFixMode.Self)]
		private Animator animator;

		[Header("Parameters")]
		[SerializeField, AnimatorParamDropdown(nameof(animator))]
		private int hitTriggerID;
		
		private void Reset()
		{
			animator = GetComponent<Animator>();
		}

		public void PlayHitAnimation()
		{
			animator.SetTrigger(hitTriggerID);
		}

		public void CollidedWithPlayer()
		{
			PlayHitAnimation();
		}
	}
}