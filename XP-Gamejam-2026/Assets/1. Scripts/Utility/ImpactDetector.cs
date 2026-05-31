using EditorAttributes;
using UnityEngine;
using UnityEngine.Events;
using VDFramework;

namespace XPGJ2026.Utility
{
	public class ImpactDetector : BetterMonoBehaviour
	{
		public UnityEvent OnImpact;

		[SerializeField, TagDropdown]
		private string[] tags;

		private void OnCollisionEnter(Collision collision)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				if (collision.gameObject.CompareTag(tags[i]))
				{
					OnImpact.Invoke();
					return;
				}
			}
		}
	}
}