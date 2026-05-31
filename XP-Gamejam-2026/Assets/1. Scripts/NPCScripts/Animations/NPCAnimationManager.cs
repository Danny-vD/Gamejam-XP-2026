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

		[Header("Other")]
		[SerializeField, TagDropdown]
		private string playerTag;
		
		private void Reset()
		{
			animator = GetComponent<Animator>();
		}

		public void PlayHitAnimation()
		{
			animator.SetTrigger(hitTriggerID);
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.CompareTag(playerTag))
			{
				CollidedWithPlayer();
			}
		}

		public void CollidedWithPlayer()
		{
			PlayHitAnimation();
		}
	}
}