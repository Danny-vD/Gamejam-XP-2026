using FMODUnity;
using UnityEngine;

namespace XPGJ2026.Audio.ScriptableObjects.AudioEventCollections
{
	[CreateAssetMenu(fileName = "OrderedAudioEventCollection", menuName = "AudioEventCollections/Ordered")]
	public class OrderedAudioEventCollection : AbstractAudioEventCollection
	{
		[Header("Event references")]
		[SerializeField]
		private EventReference[] eventReferences;

		private int currentIndex = 0;
		
		public override EventReference GetAudioEventReference()
		{
			EventReference referenceToPlay = eventReferences[currentIndex];

			currentIndex = ++currentIndex % eventReferences.Length;

			return referenceToPlay;
		}
	}
}