using UnityEngine;
using UnityEngine.Events;
using VDFramework;

namespace XPGJ2026.MenuScripts.Pages
{
	public class Page : BetterMonoBehaviour
	{
		public UnityEvent OnShow;
		public UnityEvent OnHide;

		[SerializeField]
		private bool enableDisableGameObjectOnShowHide = true;
		
		public bool IsShown { get; private set; }

		public void Show()
		{
			IsShown = true;
            
			if (enableDisableGameObjectOnShowHide)
			{
				gameObject.SetActive(true);
			}

			OnShow.Invoke();
		}

		public void Hide()
		{
			IsShown = false;
            
			if (enableDisableGameObjectOnShowHide)
			{
				gameObject.SetActive(false);
			}

			OnHide.Invoke();
		}
	}
}