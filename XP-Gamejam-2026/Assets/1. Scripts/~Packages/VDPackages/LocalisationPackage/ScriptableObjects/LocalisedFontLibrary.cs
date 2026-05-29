using EditorAttributes;
using TMPro;
using UnityEngine;
using VDPackages.LocalisationPackage.Core;
using VDPackages.LocalisationPackage.Core.Enums;
using VDPackages.SerializableDictionaryPackage.SerializableDictionary;

namespace VDPackages.LocalisationPackage.ScriptableObjects
{
	[CreateAssetMenu(fileName = "LocalisedFontLibrary", menuName = "Localisation/Fonts/LocalisedFontLibrary")]
	public class LocalisedFontLibrary : ScriptableObject
	{
		[SerializeField, TabGroup(nameof(textMeshPro), nameof(legacy))]
		private Void fontTabs;

		
		[SerializeField, VerticalGroup(nameof(fontAssetPerLanguage), nameof(fallbackFontAsset)), HideInInspector]
		private Void textMeshPro;

		[SerializeField, HideProperty]
		private SerializableDictionary<Language, TMP_FontAsset> fontAssetPerLanguage;

		[SerializeField, HideProperty]
		private TMP_FontAsset fallbackFontAsset;


		[SerializeField, VerticalGroup(nameof(fontPerLanguage), nameof(fallbackFont)), HideInInspector]
		private Void legacy;

		[SerializeField, HideProperty]
		private SerializableDictionary<Language, Font> fontPerLanguage;

		[SerializeField, HideProperty]
		private Font fallbackFont;

		public TMP_FontAsset GetFontAssetForCurrentLanguage()
		{
			if (!fontAssetPerLanguage.TryGetValue(LanguageSettings.Language, out TMP_FontAsset fontAsset))
			{
				fontAsset = fallbackFontAsset;
			}

			return fontAsset;
		}
        
		public Font GetFontForCurrentLanguage()
		{
			if (!fontPerLanguage.TryGetValue(LanguageSettings.Language, out Font font))
			{
				font = fallbackFont;
			}

			return font;
		}
	}
}