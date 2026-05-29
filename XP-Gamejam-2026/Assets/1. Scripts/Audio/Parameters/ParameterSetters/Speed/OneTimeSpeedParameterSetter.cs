using FMOD.Studio;
using XPGJ2026.Audio.Parameters.ParameterSetters.BaseClasses;
using XPGJ2026.Audio.Utility;

namespace XPGJ2026.Audio.Parameters.ParameterSetters.Speed
{
	public class OneTimeSpeedParameterSetter : AbstractParameterSetter<MovingAudioParameterValues>
	{
		public override void SetParameter(EventInstance eventInstance)
		{
			eventInstance.setParameterByName(AudioParameterUtil.SPEED_PARAMETER_NAME, audioParameterValues.GetNormalizedSpeedValue());
		}
	}
}