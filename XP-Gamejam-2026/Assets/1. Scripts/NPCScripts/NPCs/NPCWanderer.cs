using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using VDFramework;
using VDFramework.Extensions;
using VDFramework.RandomWrapper;

namespace XPGJ2026.NPCScripts.NPCs
{
	/// <summary>
	/// Walks to another random point in a predetermined list when reaching the destination
	/// </summary>
	public class NPCWanderer : BetterMonoBehaviour
	{
		public UnityEvent OnDestinationReached;
		
		[SerializeField]
		private Transform[] availableTargets;

		[SerializeField]
		private NavMeshAgent agent;

		[SerializeField]
		private float targetReachedDistance = 0.5f;

		private int currentTargetIndex = -1;
		
		private void Reset()
		{
			agent = GetComponent<NavMeshAgent>();
		}

		public void Initialise(Transform[] possiblePoints)
		{
			availableTargets = possiblePoints;
		}

		private void Start()
		{
			GoToNextTarget();
		}

		private void Update()
		{
			if (agent.remainingDistance <= targetReachedDistance)
			{
				OnDestinationReached.Invoke();
				GoToNextTarget();
			}
		}

		private void GoToNextTarget()
		{
			Transform randomElement = availableTargets.GetRandomElement(UnityRandom.StaticInstance, out currentTargetIndex, currentTargetIndex);

			agent.SetDestination(randomElement.position);
		}
	}
}