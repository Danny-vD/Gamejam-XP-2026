using System;
using UnityEngine;

namespace VDPackages.SavePackage.SavableFiles.BaseClasses
{
	/// <summary>
	/// <para>Used as a base class for <see cref="AbstractSavableFile"/></para>
	/// <para>It is a bare-bones class that only contains <see cref="Type"/> to serialize type information</para>
	/// </summary>
	[Serializable]
	public class FileTypeHolderBase : IEquatable<FileTypeHolderBase>
	{
		[SerializeField]
		private string type;
		
		private Type internalType;

		public string TypeString => type;
		
		public Type Type
		{
			get => internalType ??= GetType();
			set
			{
				internalType = value;
				type = internalType.AssemblyQualifiedName;
			}
		}

		protected FileTypeHolderBase()
		{
			Type = GetType();
		}

		public bool Equals(FileTypeHolderBase other)
		{
			return other != null && Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != this.GetType())
			{
				return false;
			}

			return Equals((FileTypeHolderBase)obj);
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}
	}
}