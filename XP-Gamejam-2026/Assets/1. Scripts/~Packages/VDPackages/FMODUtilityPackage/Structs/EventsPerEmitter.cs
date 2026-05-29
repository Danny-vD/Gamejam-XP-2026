using System;
using FMODUnity;
using UnityEngine;
using VDFramework.Interfaces;
using VDPackages.FMODUtilityPackage.Enums;

namespace VDPackages.FMODUtilityPackage.Structs
{
	[Serializable]
	public struct EventsPerEmitter : IKeyValuePair<GlobalEmitter, EventReference>
	{
		[SerializeField]
		private GlobalEmitter key;

		[SerializeField]
		private EventReference value;

		public GlobalEmitter Key
		{
			get => key;
			set => key = value;
		}

		public EventReference Value
		{
			get => value;
			set => this.value = value;
		}

		public bool Equals(IKeyValuePair<GlobalEmitter, EventReference> other)
		{
			return other != null && other.Key == Key;
		}
	}
}