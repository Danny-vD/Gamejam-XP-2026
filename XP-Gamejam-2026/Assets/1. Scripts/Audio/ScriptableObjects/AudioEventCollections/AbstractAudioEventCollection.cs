using FMODUnity;
using UnityEngine;

namespace XPGJ2026.Audio.ScriptableObjects.AudioEventCollections
{
	/// <summary>
	/// A collection of FMOD EventReferences
	/// </summary>
	/// <remarks>Specific implementations can return an event in different ways (randomly, in order, etc.)</remarks>
	public abstract class AbstractAudioEventCollection : ScriptableObject // NOTE: To make things dependant on the savefile just check it lazily, when first-time trying to get the event (date: 06-03)
	{
		public abstract EventReference GetAudioEventReference();
	}
}