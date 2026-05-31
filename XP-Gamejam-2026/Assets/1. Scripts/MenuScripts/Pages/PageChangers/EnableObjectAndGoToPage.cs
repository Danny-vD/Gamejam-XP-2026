using EditorAttributes;
using UnityEngine;
using VDFramework;

namespace XPGJ2026.MenuScripts.Pages.PageChangers
{
	public class EnableObjectAndGoToPage : BetterMonoBehaviour
	{
		[SerializeField, Required]
		private GameObject objectToEnable;

		[SerializeField, Required]
		private PageManager pageManager;

		[SerializeField, Required]
		private Page page;

		private void Reset()
		{
			pageManager    = FindAnyObjectByType<PageManager>(FindObjectsInactive.Include);
			objectToEnable = pageManager.transform.root.gameObject;
		}

		[ContextMenu("Go to page")]
		public void GoToPage()
		{
			objectToEnable.SetActive(true);
            
			pageManager.GoToPage(page);
		}
	}
}