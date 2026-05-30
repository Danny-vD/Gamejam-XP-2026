using System;
using UnityEngine;

namespace XPGJ2026.Bootstrapping.Structs
{
	[Serializable]
	public struct ObjectWithTransform
	{
		public GameObject Prefab;
		public Vector3 WorldPosition;
        public Quaternion Orientation;
	}
}