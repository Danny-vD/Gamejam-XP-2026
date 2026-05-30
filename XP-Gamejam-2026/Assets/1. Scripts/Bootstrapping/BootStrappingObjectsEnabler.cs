using UnityEngine;
using VDFramework;

namespace XPGJ2026.Bootstrapping
{
	public class BootStrappingObjectsEnabler : BetterMonoBehaviour
	{
		[SerializeField]
		private bool editorOnly;

		[Space]
		[SerializeField]
		private GameObject[] objectsToEnable;

		private void Awake()
		{
			switch (editorOnly)
			{
				case true when !Application.isEditor:
				case false when Application.isEditor:
					return;
			}

			foreach (GameObject obj in objectsToEnable)
			{
				obj.SetActive(true);
			}
		}
	}
}