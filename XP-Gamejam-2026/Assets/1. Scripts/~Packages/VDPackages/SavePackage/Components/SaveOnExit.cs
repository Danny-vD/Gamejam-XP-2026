using UnityEngine;
using VDFramework;
using VDPackages.SavePackage.FileManagement;

namespace VDPackages.SavePackage.Components
{
	/// <summary>
	/// Helper component to automatically save files
	/// </summary>
	[DefaultExecutionOrder(int.MinValue)] // Ensure saving happens before others can be destroyed
	public class SaveOnExit : BetterMonoBehaviour
	{
		[SerializeField, Tooltip("If true, all files will be saved instead of just the current ones")]
		private bool saveAll = false;

		private void Start()
		{
			if (transform.parent == null)
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
#if !UNITY_EDITOR
			if (!didStart)
			{
				return;
			}
			
			if (!hasFocus)
			{
				Save();
			}
#endif
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!didStart)
			{
				return;
			}

			if (pauseStatus)
			{
				Save();
			}
		}

		private void OnApplicationQuit()
		{
			if (!didStart)
			{
				return;
			}

			Save();
		}

		private void Save()
		{
			if (saveAll)
			{
				FileManager.SaveAllFiles();
			}
			else
			{
				FileManager.SaveAllCurrentFiles();
			}
		}
	}
}