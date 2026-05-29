using JetBrains.Annotations;
using UnityEngine;

namespace XPGJ2026.Utility
{
	public static class ScreenPositionUtil
	{
		[Pure]
		public static Vector2 GetNormalizedScreenPosition(Vector3 worldPosition, Camera camera)
		{
			Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);

			float normalizedX = screenPoint.x / Screen.width;
			float normalizedY = screenPoint.y / Screen.height;
			
			return new Vector2(normalizedX, normalizedY);
		}
	}
}