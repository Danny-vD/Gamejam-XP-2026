using FMODUnity;
using UnityEngine;
using VDFramework.Extensions;
using VDFramework.RandomWrapper;

namespace XPGJ2026.Audio.ScriptableObjects.AudioEventCollections
{
	[CreateAssetMenu(fileName = "RandomAudioEventCollection", menuName = "AudioEventCollections/Random")]
	public class RandomAudioEventCollection : AbstractAudioEventCollection
	{
		[Header("Event references")]
		[SerializeField]
		private EventReference[] eventReferences;

		[Header("Settings")]
		[SerializeField]
		private bool preventReptition = true;

		private int lastIndex = -1;
		
		public override EventReference GetAudioEventReference()
		{
			if (eventReferences.Length > 1)
			{
				if (preventReptition)
				{
					return eventReferences.GetRandomElement(out lastIndex, lastIndex);
				}

				return eventReferences.GetRandomElement(UnityRandom.StaticInstance);
			}

			return eventReferences[0];
		}
	}
}