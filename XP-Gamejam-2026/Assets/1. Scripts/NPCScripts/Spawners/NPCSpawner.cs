using System;
using UnityEngine;
using VDFramework;
using VDFramework.Extensions;
using VDFramework.RandomWrapper;
using XPGJ2026.NPCScripts.NPCs;
using XPGJ2026.SpawnerSystem.Interfaces;

namespace XPGJ2026.NPCScripts.Spawners
{
	public class NPCSpawner : BetterMonoBehaviour, ISpawner
	{
		public event Action OnSpawned;
		
		[SerializeField]
		private GameObject[] npcPrefabs;

		[SerializeField]
		private Transform[] possiblePoints;

		private int previousSpawnedIndex = -1;

		public void Spawn()
		{
			GameObject instance = Instantiate(npcPrefabs.GetRandomElement(UnityRandom.StaticInstance, out previousSpawnedIndex, previousSpawnedIndex));

			NPCWanderer npcWanderer = instance.GetComponent<NPCWanderer>();
			
			npcWanderer.Initialise(possiblePoints);
			
			OnSpawned?.Invoke();
		}
	}
}