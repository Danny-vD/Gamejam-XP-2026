using FMOD.Studio;
using FMODUnity;
using VDFramework.ObserverPattern;
using VDPackages.FMODUtilityPackage.Interfaces;

namespace XPGJ2026.Audio.Interfaces
{
	public interface IAudioEventInstancePlayer : IAudioplayer
	{
		/// <summary>
		/// Will be invoked just before the instance is played, to allow other systems to set parameters etc.
		/// </summary>
		public PrioritisedAction<EventInstance> BeforePlaying { get; }

		/// <summary>
		/// Tests if the EventReference is null
		/// </summary>
		public bool IsNull { get; }

		/// <summary>
		/// Tests if this event instance player is currently playing audio
		/// </summary>
		public bool IsPlaying { get; }

		/// <summary>
		/// Sets the audio event that this EventInstancePlayer plays
		/// </summary>
		/// <param name="newEventReference">The new audio event to play</param>
		public void SetEventReference(EventReference newEventReference);
		
		public void Cleanup(bool stopPlaying = true);
	}
}