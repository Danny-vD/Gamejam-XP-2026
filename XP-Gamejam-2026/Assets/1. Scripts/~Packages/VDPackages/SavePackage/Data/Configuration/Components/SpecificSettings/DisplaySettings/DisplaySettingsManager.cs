using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDPackages.LocalisationPackage.Core;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.DisplaySettings
{
	public class DisplaySettingsManager : BetterMonoBehaviour
	{
		[SerializeField]
		private TMP_Dropdown targetFrameRateDropdown;

		[SerializeField]
		private Toggle vSyncToggle;

		[Header("TargetFrameRate Settings")]
		[SerializeField]
		private int[] targetFrameRates = new int[] { 30, 40, 60, 90, 120, 144, 165, 170, 180, 200 };

		[SerializeField]
		private string frameRateLabel = "{0} FPS";

		[SerializeField, Prefix("localised")]
		private string unlimitedLabel = "UNLIMITED/0";

		private int unlimitedIndex;

		private void Reset()
		{
			targetFrameRateDropdown = GetComponentInChildren<TMP_Dropdown>();

			vSyncToggle = GetComponentInChildren<Toggle>();
		}

		private void Awake()
		{
			FillTargetFrameRateDropdown();
		}

		private void OnEnable()
		{
			SetCurrentOptions();

			targetFrameRateDropdown.options[unlimitedIndex] = new TMP_Dropdown.OptionData(GetUnlimitedOption());
			targetFrameRateDropdown.RefreshShownValue();

			targetFrameRateDropdown.onValueChanged.AddListener(OnSelectedFrameRateOption);
			vSyncToggle.onValueChanged.AddListener(OnToggleVsync);
		}

		private void OnDisable()
		{
			vSyncToggle.onValueChanged.RemoveListener(OnToggleVsync);
			targetFrameRateDropdown.onValueChanged.RemoveListener(OnSelectedFrameRateOption);
		}

		private void FillTargetFrameRateDropdown()
		{
			List<string> options = GetFixedOptions();
			unlimitedIndex = options.Count;

			options.Add(GetUnlimitedOption());

			targetFrameRateDropdown.ClearOptions();
			targetFrameRateDropdown.AddOptions(options);

			targetFrameRateDropdown.RefreshShownValue();
		}

		private List<string> GetFixedOptions()
		{
#if UNITY_IOS || UNITY_ANDROID
			return targetFrameRates.Where(value => value <= Screen.currentResolution.refreshRateRatio.value).Select(value => string.Format(frameRateLabel, value)).ToList();
#else
			return targetFrameRates.Select(value => string.Format(frameRateLabel, value)).ToList();
#endif
		}

		private string GetUnlimitedOption()
		{
			return LocalisationUtil.GetLocalisedStringNested(unlimitedLabel, "[");
		}

		private void SetCurrentOptions()
		{
			int currentTarget = Application.targetFrameRate;

			if (currentTarget <= 0)
			{
				targetFrameRateDropdown.SetValueWithoutNotify(unlimitedIndex);
			}
			else
			{
				bool targetFound = false;

				for (int i = 0; i < targetFrameRates.Length; i++)
				{
					int targetFrameRate = targetFrameRates[i];

					if (targetFrameRate == currentTarget)
					{
						targetFound = true;

						targetFrameRateDropdown.SetValueWithoutNotify(i);
						break;
					}
				}

				if (!targetFound)
				{
					targetFrameRateDropdown.SetValueWithoutNotify(unlimitedIndex);
					
					Debug.LogError($"A target framerate of {currentTarget} is not in the dropdown!");
				}
			}

			vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
		}

		private void OnSelectedFrameRateOption(int index)
		{
			if (index == unlimitedIndex)
			{
				Application.targetFrameRate = -1;
				return;
			}

			Application.targetFrameRate = targetFrameRates[index];
		}

		private static void OnToggleVsync(bool enableVSync)
		{
			QualitySettings.vSyncCount = enableVSync ? 1 : 0;
		}
	}
}