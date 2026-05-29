using UnityEngine;
using VDFramework.Singleton;
using VDPackages.LocalisationPackage.ScriptableObjects;

namespace VDPackages.LocalisationPackage.Components.Font
{
	public class MainFontLibraryHolder : Singleton<MainFontLibraryHolder>
	{
		[SerializeField, Tooltip("Any LocalisedFont will fallback to this library if they have none set")]
		private LocalisedFontLibrary localisedFontLibrary;

		public LocalisedFontLibrary GetFontLibrary()
		{
			return localisedFontLibrary;
		}
	}
}