using UnityEngine;
using UnityEngine.UI;
using VDPackages.SerializableDictionaryPackage.SerializableDictionary;
using VDPackages.UtilityPackage.CursorManagement.CursorUtility;
using VDPackages.UtilityPackage.CursorManagement.CursorUtility.Singletons;
using VDPackages.UtilityPackage.CursorManagement.Structs;

namespace VDPackages.UtilityPackage.CursorManagement.CursorComponents
{
	public class HoverCursorComponent : AbstractCursorComponent
	{
		[SerializeField, Tooltip("The CursorData to use if no other data is specified")]
		private CursorData defaultHoverDatum;

		[SerializeField, Tooltip("Specify a CursorData for a specific tag")]
		private SerializableDictionary<string, CursorData> tagData;

		public override bool IsAdditiveEffect => false;

		private bool pointerIsHoveringOverSelectable = false;

		private CursorData? cursorDataToSet;

		protected override void OnDeactivate()
		{
			cursorDataToSet = null;
		}

		public override bool AreConditionsMet()
		{
			if (IsPointerOverSelectable(out GameObject hoveredSelectableObject))
			{
				CursorData newCursorData = GetCursorData(hoveredSelectableObject);

				if (!newCursorData.Equals(cursorDataToSet)) // Prevent updating to the cursorData that is already set
				{
					cursorDataToSet    = newCursorData;
					ShouldUpdateCursor = true;
				}
			}
			
			return pointerIsHoveringOverSelectable;
		}

		public override CursorData GetCursorData()
		{
			ShouldUpdateCursor = false;
			return cursorDataToSet!.Value;
		}

		private bool IsPointerOverSelectable(out GameObject hoveredSelectableObject)
		{
			pointerIsHoveringOverSelectable = false;
			hoveredSelectableObject         = null;
			
			if (CursorUtil.Instance.TryGetHoveredGameObject(out GameObject hoveredGameObject))
			{
				if (hoveredGameObject.GetComponent<Selectable>() != null)
				{
					pointerIsHoveringOverSelectable = true;
					hoveredSelectableObject         = hoveredGameObject;
				}
			}

			return pointerIsHoveringOverSelectable;
		}

		private CursorData GetCursorData(GameObject hoveredObject)
		{
			if (hoveredObject.TryGetComponent(out CursorTextureComponent cursorTexture))
			{
				return cursorTexture.CursorData;
			}

			if (tagData.TryGetValue(hoveredObject.tag, out CursorData cursorData))
			{
				return cursorData;
			}

			return defaultHoverDatum;
		}
	}
}