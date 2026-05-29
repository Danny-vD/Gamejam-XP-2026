using UnityEngine;
using UnityEngine.UI;
using VDFramework;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.InputSettings
{
	public class InputSettingsManager : BetterMonoBehaviour
	{
		[SerializeField]
		private Toggle confinedCursorToggle;

		private void OnEnable()
		{
			confinedCursorToggle.onValueChanged.AddListener(OnConfinedCursorChanged);
		}

		private void OnDisable()
		{
			confinedCursorToggle.onValueChanged.RemoveListener(OnConfinedCursorChanged);
		}

		private static void OnConfinedCursorChanged(bool confined)
		{
			Cursor.lockState = confined ? CursorLockMode.Confined : CursorLockMode.None;
		}
	}
}