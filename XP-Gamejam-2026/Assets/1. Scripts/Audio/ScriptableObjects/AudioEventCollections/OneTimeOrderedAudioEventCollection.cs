using FMODUnity;
using UnityEngine;

namespace XPGJ2026.Audio.ScriptableObjects.AudioEventCollections
{
	/// <summary>
	/// <para>Returns the eventReferences in order, but does not loop.</para>
	/// <para>After reaching the end it will always return the last eventReference.</para>
	/// </summary>
	[CreateAssetMenu(fileName = "OrderedAudioEventCollection", menuName = "AudioEventCollections/OneTimeOrdered")]
	public class OneTimeOrderedAudioEventCollection : AbstractAudioEventCollection
	{
		[Header("Event references")]
		[SerializeField]
		private EventReference[] eventReferences;

		private int currentIndex = 0;
		
		public override EventReference GetAudioEventReference()
		{
			EventReference referenceToPlay = eventReferences[currentIndex];

			bool reachedLast = currentIndex == eventReferences.Length - 1;
			
			if (!reachedLast)
			{
				++currentIndex;
			}

			return referenceToPlay;
		}
	}
}