using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

namespace VDPackages.UtilityPackage.Utility.CursorUtility
{
	[DisallowMultipleComponent]
	public class CursorHider : BetterMonoBehaviour
	{
		private enum CursorHideMode
		{
			AlwaysShowCursor,
			HideCursor,
			
			[UsedImplicitly]
			HideCursorAfterClick,
		}

		[SerializeField]
		private InputActionReference mouseClickInput;

		[SerializeField, Tooltip("When should the cursor be hidden?")]
		private CursorHideMode hideMode = CursorHideMode.HideCursor;

		private void OnEnable()
		{
			if (hideMode == CursorHideMode.AlwaysShowCursor)
			{
				ShowCursor();
			}
			else // HideCursorAfterClick || HideCursor
			{
				mouseClickInput.action.canceled += HideCursor;
			}

			if (hideMode == CursorHideMode.HideCursor)
			{
				HideCursor(default);
			}
		}

		private void OnDisable()
		{
			mouseClickInput.action.canceled -= HideCursor;
		}

		public static void HideCursor(InputAction.CallbackContext callbackContext)
		{
			Cursor.visible = false;
		}

		public static void ShowCursor()
		{
			Cursor.visible = true;
		}
	}
}