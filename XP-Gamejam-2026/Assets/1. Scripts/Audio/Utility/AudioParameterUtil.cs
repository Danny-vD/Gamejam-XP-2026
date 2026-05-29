using UnityEngine;
using VDFramework.Extensions;
using VDPackages.FMODUtilityPackage.Core;
using XPGJ2026.Utility;

namespace XPGJ2026.Audio.Utility
{
	public static class AudioParameterUtil
	{
		//\\//\\// Global Parameters \\//\\//\\

		public const string SPEED_PARAMETER_NAME = "speed";

		/// <summary>
		/// 0 or 1 denotes that the level only just started and some sounds should not play (respawn, impact etc)
		/// </summary>
		public const string LEVEL_START_NAME = "LevelStart";
		
		//\\//\\// Local Parameters \\//\\//\\

		public static float GetPanParameterValue(Vector3 worldPosition, Camera camera)
		{
			Vector2 normalizedScreenPosition = ScreenPositionUtil.GetNormalizedScreenPosition(worldPosition, camera);

			return normalizedScreenPosition.x * 2 - 1;
		}

		public static void SetLevelStartGlobalParameter(bool value)
		{
			AudioParameterManager.SetGlobalParameter(LEVEL_START_NAME, value.ConvertTo<float>());
		}
	}
}