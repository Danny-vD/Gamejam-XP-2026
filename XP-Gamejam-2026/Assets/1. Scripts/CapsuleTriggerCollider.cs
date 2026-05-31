using UnityEngine;

namespace XPGJ2026
{
	public class CapsuleTriggerAnimation : MonoBehaviour
	{
		private Animator animator;

		// The exact name of the trigger parameter in your Animator Controller
		[SerializeField] private string animationTriggerName = "Play";

		void Start()
		{
			animator = GetComponent<Animator>();
		}

		// Use this if "Is Trigger" is ENABLED on the collider
		void OnTriggerEnter(Collider other)
		{
			// Optional: only react to the player
			if (other.CompareTag("Player"))
			{
				animator.SetTrigger(animationTriggerName);
			}
		}

		// Use this instead if "Is Trigger" is DISABLED (physical collision)
		void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				animator.SetTrigger(animationTriggerName);
			}
		}
	}

}
