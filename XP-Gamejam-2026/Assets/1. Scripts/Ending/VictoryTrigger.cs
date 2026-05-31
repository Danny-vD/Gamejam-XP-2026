using UnityEngine;
using UnityEngine.SceneManagement;
using VDFramework;

namespace XPGJ2026.Ending
{
	public class VictoryTrigger : BetterMonoBehaviour
	{
		[SerializeField]
		private int victorySceneIndex;
		
		private void OnTriggerEnter(Collider other)
		{
			SceneManager.LoadScene(victorySceneIndex);
		}
	}
}