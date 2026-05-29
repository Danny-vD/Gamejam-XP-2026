using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;
using VDPackages.SavePackage.Data.Events;

namespace VDPackages.SavePackage.Data.Configuration.Components.SpecificSettings.GraphicsSettings
{
	public class GraphicsSettingsManager : BetterMonoBehaviour
	{
		[SerializeField]
		private FullScreenMode[] fullScreenModePerDropdownIndex;

		[Space(16)]
		[SerializeField]
		private TMP_Dropdown resolutionDropdown;

		[SerializeField]
		private TMP_Dropdown fullscreenDropdown;

		public FullScreenMode CurrentFullScreenMode { get; private set; }
		public Resolution CurrentResolution { get; private set; }

		private Resolution[] resolutions;

		private void Awake()
		{
			CurrentResolution     = Screen.currentResolution;
			CurrentFullScreenMode = Screen.fullScreenMode;
			
			resolutions = Screen.resolutions.Where(resolution => resolution.refreshRateRatio.Equals(CurrentResolution.refreshRateRatio)).ToArray();
		}

		private void Start()
		{
			InitializeResolutionDropdown();
			InitializeFullScreenDropdown();
		}

		private void OnEnable()
		{
			fullscreenDropdown.onValueChanged.AddListener(OnSelectedFullScreenMode);
			resolutionDropdown.onValueChanged.AddListener(OnSelectedResolution);
		}

		private void OnDisable()
		{
			fullscreenDropdown.onValueChanged.RemoveListener(OnSelectedFullScreenMode);
			resolutionDropdown.onValueChanged.RemoveListener(OnSelectedResolution);
		}

		private void OnSelectedFullScreenMode(int fullscreenIndex)
		{
			CurrentFullScreenMode = fullScreenModePerDropdownIndex[fullscreenIndex];

			SetDisplaySettings();
		}

		private void OnSelectedResolution(int resolutionIndex)
		{
			CurrentResolution = resolutions[resolutionIndex];

			SetDisplaySettings();
		}

		private void SetDisplaySettings()
		{
			Screen.SetResolution(CurrentResolution.width, CurrentResolution.height, CurrentFullScreenMode, CurrentResolution.refreshRateRatio);
			
			EventManager.RaiseEvent(new ResolutionChangedEvent());
		}

		private void InitializeResolutionDropdown()
		{
			resolutionDropdown.ClearOptions();

			List<string> options = new List<string>();

			int currentResolutionIndex = 0;

			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];

				options.Add(GetResolutionString(resolution));

				if (resolution.Equals(CurrentResolution))
				{
					currentResolutionIndex = i;
				}
			}

			resolutionDropdown.AddOptions(options);
			resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
			resolutionDropdown.RefreshShownValue();
		}

		private void InitializeFullScreenDropdown()
		{
			for (int i = 0; i < fullScreenModePerDropdownIndex.Length; i++)
			{
				if (fullScreenModePerDropdownIndex[i] == CurrentFullScreenMode)
				{
					fullscreenDropdown.SetValueWithoutNotify(i);
					fullscreenDropdown.RefreshShownValue();

					break;
				}
			}
		}

		private static string GetResolutionString(Resolution resolution)
		{
			return string.Format("{0} x {1} @ {2:N2}Hz", resolution.width, resolution.height, resolution.refreshRateRatio.value);
		}
	}
}