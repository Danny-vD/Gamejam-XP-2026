using EditorAttributes;
using UnityEngine;
using VDFramework;
using VDFramework.Logger;
using VDFramework.Logger.Enums;
using VDFramework.Logger.Implementations;

namespace VDPackages.UtilityPackage
{
	/// <summary>
	/// Simple class to set the <see cref="LogManager.LoggerImplementation"/> in the <see cref="LogManager"/><br/>
	/// Necessary because the LogManager is environment agnostic so it cannot default to Unity's logger
	/// </summary>
	[DefaultExecutionOrder(-1000)] // Since it sets the logger, ensure it always happens first
	public class LoggerImplementationSetter : BetterMonoBehaviour
	{
		[SerializeField, HelpBox("If true, the logging will be disabled entirely in a build", drawAbove: true)]
		private bool disableInBuild = false;

		[Space]
		[SerializeField]
		private LogLevel logLevelInEditor = LogLevel.All;

		[SerializeField]
		private LogLevel logLevelInBuild = LogLevel.Important;

		private void Awake()
		{
#if !UNITY_EDITOR
			if (disableInBuild)
			{
				LogManager.Enabled = false;
				return;
			}

			LogManager.LogLevel = logLevelInBuild;
#else
			LogManager.LogLevel = logLevelInEditor;
#endif

			LogManager.LoggerImplementation = new DebugLogger();
			Destroy(this);
		}
	}
}