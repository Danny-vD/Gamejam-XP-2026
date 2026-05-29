using UnityEngine;
using UnityEngine.Events;
using VDFramework;

namespace LittleChef.Grabbables.Components
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