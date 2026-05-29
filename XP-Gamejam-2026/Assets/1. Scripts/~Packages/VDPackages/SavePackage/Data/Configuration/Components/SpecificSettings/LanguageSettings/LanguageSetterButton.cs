using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDPackages.LocalisationPackage.Core.Enums;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.LanguageSettings
{
	public class LanguageSetterButton : BetterMonoBehaviour
	{
		[SerializeField]
		private Button button;

		[SerializeField]
		private Language language;

		private void Reset()
		{
			button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			if (VDPackages.LocalisationPackage.Core.LanguageSettings.Language == language)
			{
				button.Select();
			}
			
			button.onClick.AddListener(SetLanguage);
		}

		private void OnDisable()
		{
			button.onClick.RemoveListener(SetLanguage);
		}

		private void SetLanguage()
		{
			LocalisationPackage.Core.LanguageSettings.Language = language;
		}
	}
}