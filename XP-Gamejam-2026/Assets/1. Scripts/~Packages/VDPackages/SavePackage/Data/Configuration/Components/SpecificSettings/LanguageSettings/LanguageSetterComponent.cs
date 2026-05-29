using VDFramework;
using VDPackages.LocalisationPackage.Core.Enums;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.LanguageSettings
{
	public class LanguageSetterComponent : BetterMonoBehaviour
	{
		public void SetLanguage(Language language)
		{
			LocalisationPackage.Core.LanguageSettings.Language = language;
		}
	}
}