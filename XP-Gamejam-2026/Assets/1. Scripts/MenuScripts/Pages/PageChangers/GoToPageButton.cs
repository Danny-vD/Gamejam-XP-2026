using UnityEngine;
using UnityEngine.UI;
using VDFramework;

namespace XPGJ2026.MenuScripts.Pages.PageChangers
{
	public class GoToPageButton : BetterMonoBehaviour
	{
		[SerializeField]
		private PageManager pageManager;

		[SerializeField]
		private Page pageToGoTo;

		[Space]
		[SerializeField]
		private Button button;
		
		private void Reset()
		{
			pageManager = GetComponentInParent<PageManager>();
			pageToGoTo  = GetComponent<Page>();

			button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			button.onClick.AddListener(OnClick);
		}

		private void OnDisable()
		{
			button.onClick.RemoveListener(OnClick);
		}

		private void OnClick()
		{
			pageManager.GoToPage(pageToGoTo);
		}

		private void OnDestroy()
		{
			pageManager = null;
			pageToGoTo  = null;
		}
	}
}