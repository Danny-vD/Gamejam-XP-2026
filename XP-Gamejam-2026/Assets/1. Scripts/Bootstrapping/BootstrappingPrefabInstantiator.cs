using UnityEngine;

namespace XPGJ2026.Bootstrapping
{
	public static class BootstrappingPrefabInstantiator
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void InstantiatePrefab()
		{
			GameObject bootstrappingPrefab = Resources.Load<GameObject>("Bootstrapping/BootstrappingPrefab");

			if (bootstrappingPrefab == null)
			{
				Debug.LogError("No bootstrapping prefab found in the project!");
				return;
			}

			GameObject instance = Object.Instantiate(bootstrappingPrefab);
			instance.name = bootstrappingPrefab.name;
			
			Object.DontDestroyOnLoad(instance);
		}
	}
}