using UnityEngine;
using VDFramework;
using VDFramework.ObserverPattern;

namespace XPGJ2026.MenuScripts.Pages
{
	public class PageManager : BetterMonoBehaviour
	{
		public readonly PrioritisedAction OnSwitchedPage = new PrioritisedAction();

		[SerializeField]
		protected Page[] pages;

		[SerializeField, Tooltip("The page which will be shown the first time the manager is enabled (if null, no page will be shown)")]
		protected Page startPage;

		[SerializeField, Tooltip("If true, the manager will switch to the start page every time it is enabled (instead of only the first time on Awake)")]
		protected bool startAtStartPageEveryTime = false;

		public Page CurrentPage { get; protected set; }

		private void Reset()
		{
			pages = GetComponentsInChildren<Page>();
		}

		protected virtual void OnEnable()
		{
			if (!didStart)
			{
				return;
			}

			if (startAtStartPageEveryTime)
			{
				GoToStartPage();
			}
			else
			{
				GoToPage(CurrentPage);
			}
		}

		protected virtual void Start()
		{
			if (CurrentPage == null || startAtStartPageEveryTime)
			{
				GoToStartPage();
			}
		}

		public void GoToPage(Page gotoPage)
		{
			CurrentPage = gotoPage;

			bool containsPage = false;

			foreach (Page page in pages)
			{
				bool shouldShow = page == gotoPage;

				if (shouldShow)
				{
					containsPage = true;
				}
				else
				{
					page.Hide();
				}
			}

			if (containsPage)
			{
				gotoPage.Show();
			}

			OnSwitchedPage.Invoke();
		}

		public virtual void GoToStartPage()
		{
			GoToPage(startPage);
		}

		protected virtual void OnDestroy()
		{
			pages     = null;
			startPage = null;

			OnSwitchedPage.ClearCallbacks();
		}
	}
}