using UnityEngine;
using UnityEngine.Events;
using VDFramework;

namespace XPGJ2026.Utility
{
	public class ImpactDetector : BetterMonoBehaviour
	{
		public UnityEvent OnImpact;
		
		private void OnCollisionEnter2D(Collision2D _)
		{
			OnImpact.Invoke();
		}
	}
}