using UnityEngine;
using UnityEngine.Animations;

namespace XPGJ2026.NPCScripts.StateMachineBehaviours
{
	public class NPCAnimation : StateMachineBehaviour
	{
		[SerializeField]
		private float height;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			Transform transformParent = animator.transform;

			Vector3 currentpos = transformParent.position;

			currentpos.y -= height;

			transformParent.position = currentpos;
		}
	}
}