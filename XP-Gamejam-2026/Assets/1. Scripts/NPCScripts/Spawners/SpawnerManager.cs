using System.Linq;
using AYellowpaper;
using UnityEngine;
using VDFramework;
using VDFramework.Extensions;
using VDFramework.RandomWrapper;
using VDFramework.Timer;
using VDFramework.Timer.TimerHandles;
using XPGJ2026.SpawnerSystem.Interfaces;

namespace XPGJ2026.NPCScripts.Spawners
{
	public class SpawnerManager : BetterMonoBehaviour
	{
		[Header("Spawners")]
		[SerializeField]
		private InterfaceReference<ISpawner>[] spawners;

		[Header("Settings")]
		[SerializeField]
		private int maximumSpawnedCount = 20;

		[SerializeField]
		private float spawnDelaySeconds = 2.8f;

		[Space]
		[SerializeField]
		private bool spawnImmediately = true;

		public int CurrentSpawnedCount { get; private set; } = 0;

		private int previousSpawnerIndex = -1;

		private TimerHandle spawnTimerHandle;

		private void Reset()
		{
			spawners = GetComponents<ISpawner>().Select(spawner => new InterfaceReference<ISpawner>() { Value = spawner }).ToArray();
		}

		private void Awake()
		{
			spawnTimerHandle = new TimerHandle(spawnDelaySeconds, SpawnFromSpawner, true);
		}

		private void Start()
		{
			if (spawnImmediately)
			{
				SpawnFromSpawner();
			}

			TimerManager.StartTimer(spawnTimerHandle);
		}

		public void SpawnFromSpawner()
		{
			if (CurrentSpawnedCount >= maximumSpawnedCount)
			{
				spawnTimerHandle?.Stop();
				spawnTimerHandle = null;
				return;
			}

			ISpawner spawner = spawners.GetRandomElement(UnityRandom.StaticInstance, out previousSpawnerIndex, previousSpawnerIndex).Value;
			spawner.Spawn();

			++CurrentSpawnedCount;
		}
	}
}