using UnityEngine;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using VDFramework.Singleton;

namespace XPGJ2026
{
	public class AudioManager : Singleton<AudioManager>
	{
		[SerializeField] private EventReference musicEventRef; // Assign in Inspector

		private EventInstance musicInstance;

		void Start()
		{
			PlayMusic();
		}

		void PlayMusic()
		{
			musicInstance = RuntimeManager.CreateInstance(musicEventRef);
			musicInstance.start();
		}

		void StopMusic(bool fadeOut = true)
		{
			musicInstance.stop(fadeOut
				? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
				: FMOD.Studio.STOP_MODE.IMMEDIATE);
			musicInstance.release();
		}

		public void SetMusicArea(MusicArea area)
		{
			musicInstance.setParameterByName("areas",(float)area);
		}
		void OnDestroy()
		{
			StopMusic();
		}
	}
}
