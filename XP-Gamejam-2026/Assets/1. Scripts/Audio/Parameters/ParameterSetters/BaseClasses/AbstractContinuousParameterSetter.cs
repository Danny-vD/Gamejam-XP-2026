using System.Collections;
using AYellowpaper;
using FMOD.Studio;
using VDPackages.FMODUtilityPackage.Core;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.Parameters.ParameterSetters.BaseClasses
{
	public abstract class AbstractContinuousParameterSetter<TAudioParameterValues> : AbstractParameterSetter<TAudioParameterValues> where TAudioParameterValues : GenericAudioParameterValues
	{
		protected override void OnEnable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.AddCallback(SetParameter);
			}
		}

		protected override void OnDisable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.RemoveCallback(SetParameter);
			}
		}

		private void StartSetParameterEveryFrame(EventInstance eventInstance)
		{
			StartCoroutine(SetParameterEveryFrame(eventInstance));
		}

		private IEnumerator SetParameterEveryFrame(EventInstance eventInstance)
		{
			do
			{
				SetParameter(eventInstance);

				yield return null;
			} while (AudioPlayer.IsPlaying(eventInstance));
		}
	}

	public abstract class AbstractContinuousParameterSetter : AbstractParameterSetter
	{
		protected override void OnEnable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.AddCallback(StartSetParameterEveryFrame);
			}
		}

		protected override void OnDisable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.RemoveCallback(StartSetParameterEveryFrame);
			}
		}

		protected virtual void StartSetParameterEveryFrame(EventInstance eventInstance)
		{
			StartCoroutine(SetParameterEveryFrame(eventInstance));
		}

		private IEnumerator SetParameterEveryFrame(EventInstance eventInstance)
		{
			do
			{
				SetParameter(eventInstance);

				yield return null;
			} while (AudioPlayer.IsPlaying(eventInstance));
		}
	}
}