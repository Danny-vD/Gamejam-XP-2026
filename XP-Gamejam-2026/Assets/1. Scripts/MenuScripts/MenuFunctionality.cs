using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VDFramework;

namespace XPGJ2026.MenuScripts
{
	public class MenuFunctionality : BetterMonoBehaviour
	{
		public void LoadScene(int index)
		{
			SceneManager.LoadScene(index);
		}

		public void QuitApplication()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
			return;
#endif
			Application.Quit();
		}
	}
}