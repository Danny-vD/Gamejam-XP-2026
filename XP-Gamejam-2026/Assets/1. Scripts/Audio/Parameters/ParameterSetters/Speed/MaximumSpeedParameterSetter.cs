using FMOD.Studio;
using XPGJ2026.Audio.Parameters.ParameterSetters.BaseClasses;
using XPGJ2026.Audio.Utility;

namespace XPGJ2026.Audio.Parameters.ParameterSetters.Speed
{
	/// <summary>
	/// Sets the <see cref="AudioParameterUtil.SPEED_PARAMETER_NAME"/> parameter to 1 and never changes it
	/// </summary>
	public class MaximumSpeedParameterSetter : AbstractParameterSetter
	{
		public override void SetParameter(EventInstance eventInstance)
		{
			eventInstance.setParameterByName(AudioParameterUtil.SPEED_PARAMETER_NAME, 1);
		}
	}
}