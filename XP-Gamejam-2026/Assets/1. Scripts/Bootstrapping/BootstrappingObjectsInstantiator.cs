using UnityEngine;
using VDFramework;
using XPGJ2026.Bootstrapping.Enums;
using XPGJ2026.Bootstrapping.Structs;

namespace XPGJ2026.Bootstrapping
{
	public class BootstrappingObjectsInstantiator : BetterMonoBehaviour
	{
		[SerializeField]
		private ExecutionEnvironment whenToInstantiate = ExecutionEnvironment.EditorAndBuild;

		[Space]
		[SerializeField]
		private ObjectWithTransform[] objectsToInstantiate;

		private void Awake()
		{
			switch (whenToInstantiate)
			{
				case ExecutionEnvironment.EditorOnly when !Application.isEditor:
				case ExecutionEnvironment.BuildOnly when Application.isEditor:
					return;
			}

			foreach (ObjectWithTransform objectWithTransform in objectsToInstantiate)
			{
				GameObject instance = Instantiate(objectWithTransform.Prefab, objectWithTransform.WorldPosition, objectWithTransform.Orientation);
				
				instance.name = objectWithTransform.Prefab.name;
			}
		}
	}
}