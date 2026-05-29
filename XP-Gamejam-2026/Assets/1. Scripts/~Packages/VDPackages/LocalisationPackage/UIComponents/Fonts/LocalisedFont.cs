using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDFramework.EventSystem;
using VDPackages.LocalisationPackage.Components.Font;
using VDPackages.LocalisationPackage.Events;
using VDPackages.LocalisationPackage.ScriptableObjects;

namespace VDPackages.LocalisationPackage.UIComponents.Fonts
{
	public class LocalisedFont : BetterMonoBehaviour
	{
		private Text labelLegacy;
		private TMP_Text labelTMP;

		[SerializeField, Prefix("Optional"), Tooltip("If not set it will use the one from the main font library object")]
		private LocalisedFontLibrary localisedFontLibrary;

		private LocalisedFontLibrary fontLibrary;

		private void Awake()
		{
			labelLegacy = GetComponent<Text>();
			labelTMP    = GetComponent<TMP_Text>();
            
            fontLibrary = localisedFontLibrary ?? MainFontLibraryHolder.Instance.GetFontLibrary();
		}

		private void OnEnable()
		{
			if (didStart)
			{
				LocaliseFontToCurrentLanguage();
			}
			
			EventManager.AddListener<LanguageChangedEvent>(LocaliseFontToCurrentLanguage);
		}

		private void OnDisable()
		{
			EventManager.RemoveListener<LanguageChangedEvent>(LocaliseFontToCurrentLanguage);
		}

		private void Start()
		{
			LocaliseFontToCurrentLanguage();
		}

		public void SetFontLibrary(LocalisedFontLibrary library)
		{
			fontLibrary = library;
			LocaliseFontToCurrentLanguage();
		}

		public void LocaliseFontToCurrentLanguage()
		{
			if (labelTMP)
			{
				labelTMP.font = fontLibrary.GetFontAssetForCurrentLanguage();
			}
			else if (labelLegacy)
			{
				labelLegacy.font = fontLibrary.GetFontForCurrentLanguage();
			}
		}
	}
}