using FMOD.Studio;

namespace VDPackages.FMODUtilityPackage.Interfaces
{
	/// <summary>
	/// An interface that provides basic functions to Play, Pause and Stop audio.
	/// </summary>
	public interface IAudioplayer
	{
		/// <summary>
		/// Play the audio
		/// </summary>
		void Play();

		/// <summary>
		/// Play the audio if the audio is not already playing
		/// </summary>
		void PlayIfNotPlaying();

		/// <summary>
		/// Stop playing the audio
		/// </summary>
		public void Stop()
		{
			Stop(STOP_MODE.ALLOWFADEOUT);
		}
		
		/// <summary>
		/// Stop playing the audio without allowing fadeout
		/// </summary>
		public void StopImmediately()
		{
			Stop(STOP_MODE.IMMEDIATE);
		}
		
		public void Stop(STOP_MODE stopMode);

		/// <summary>
		/// Set the pause state of the audio
		/// </summary>
		void SetPause(bool paused);
	}
}