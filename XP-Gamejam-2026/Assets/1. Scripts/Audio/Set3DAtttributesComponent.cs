using System;
using AYellowpaper;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using VDPackages.FMODUtilityPackage.Core;
using XPGJ2026.Audio.EventInstancePlayers;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026
{
    public class Set3DAtttributesComponent : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IAudioEventInstancePlayer> audioPlayer;
		[SerializeField] private InterfaceReference<IAudioEventInstancePlayer> audioPlayerShoot;

		private Rigidbody rigidbdy;

		private void Awake()
		{
			rigidbdy = GetComponent<Rigidbody>();
		}

		private void OnEnable()
		{
			audioPlayer.Value.BeforePlaying.AddCallback(played);
			audioPlayerShoot.Value.BeforePlaying.AddCallback(played);
		}

		private void OnDisable()
		{
			audioPlayer.Value.BeforePlaying.RemoveCallback(played);
			audioPlayerShoot.Value.BeforePlaying.RemoveCallback(played);
		}

		private void played(EventInstance instance)
		{
			RuntimeManager.AttachInstanceToGameObject(instance, gameObject, rigidbdy);
			
			ATTRIBUTES_3D attributes3D = gameObject.To3DAttributes();
			instance.set3DAttributes(attributes3D);
		}
	}
}
