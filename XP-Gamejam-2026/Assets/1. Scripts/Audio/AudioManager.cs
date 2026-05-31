using UnityEngine;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace XPGJ2026
{
	public class AudioManager : MonoBehaviour
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

		void OnDestroy()
		{
			StopMusic();
		}
	}
}
