using AYellowpaper;
using FMOD.Studio;
using UnityEngine;
using VDFramework;
using VDFramework.Logger;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.Parameters.ParameterSetters.BaseClasses
{
	[DefaultExecutionOrder(-1)] // Ensure this always happens before everone else so we can subscribe to events that are played OnEnable
	public abstract class AbstractParameterSetter : BetterMonoBehaviour
	{
		[SerializeField]
		protected InterfaceReference<IAudioEventInstancePlayer>[] eventPlayers;

		protected virtual void Reset()
		{
			IAudioEventInstancePlayer[] audioEventInstancePlayers = GetComponents<IAudioEventInstancePlayer>();

			eventPlayers = new InterfaceReference<IAudioEventInstancePlayer>[audioEventInstancePlayers.Length];
			
			for (int i = 0; i < audioEventInstancePlayers.Length; i++)
			{
				eventPlayers[i] = new InterfaceReference<IAudioEventInstancePlayer>() { Value = audioEventInstancePlayers[i] };
			}
		}

		protected virtual void OnEnable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.AddCallback(SetParameter);
			}
		}

		protected virtual void OnDisable()
		{
			foreach (InterfaceReference<IAudioEventInstancePlayer> interfaceReference in eventPlayers)
			{
				interfaceReference.Value.BeforePlaying.RemoveCallback(SetParameter);
			}
		}

		public abstract void SetParameter(EventInstance eventInstance);
	}

	public abstract class AbstractParameterSetter<TAudioParameterValues> : AbstractParameterSetter where TAudioParameterValues : GenericAudioParameterValues
	{
		[SerializeField]
		protected TAudioParameterValues audioParameterValues;

		protected override void Reset()
		{
			base.Reset();
			audioParameterValues = GetComponentInParent<TAudioParameterValues>();
		}

		protected virtual void Awake()
		{
			if (audioParameterValues == null)
			{
				LogManager.LogError($"No {nameof(TAudioParameterValues)} set!", this);
			}
		}
	}
}