using System;

namespace XPGJ2026.SpawnerSystem.Interfaces
{
	public interface ISpawner
	{
		public event Action OnSpawned;
		
		public void Spawn();
	}
}