using EditorAttributes;
using UnityEngine;
using VDFramework;

namespace XPGJ2026.MenuScripts.Pages.PageChangers
{
	public class GoToPageComponent : BetterMonoBehaviour
	{
		[SerializeField, Required]
		private PageManager pageManager;

		[SerializeField, Required]
		private Page page;

		private void Reset()
		{
			pageManager    = FindAnyObjectByType<PageManager>(FindObjectsInactive.Include);
		}

		[ContextMenu("Go to page")]
		public void GoToPage()
		{
			pageManager.GoToPage(page);
		}
	}
}