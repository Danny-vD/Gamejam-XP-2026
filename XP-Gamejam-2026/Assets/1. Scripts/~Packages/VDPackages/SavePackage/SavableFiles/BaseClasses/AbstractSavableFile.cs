using System;
using UnityEngine;
using VDPackages.SavePackage.Parsing;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SaveDirectory;

namespace VDPackages.SavePackage.SavableFiles.BaseClasses
{
	/// <summary>
	/// <para>The base class of all files that can be saved</para>
	/// <para>To know which specific type a serialized file belongs to, you can deserialize into <see cref="FileTypeHolderBase"/> first<br/>
	/// (Or when writing your own parser, deserialize only the string that holds the type and check that)</para>
	/// </summary>
	[Serializable]
	public abstract class AbstractSavableFile : FileTypeHolderBase, IEquatable<AbstractSavableFile>
	{
		public event Action OnBecomeDirty = null;
		public event Action OnBecomeClean = null;
		public event Action OnDataChanged = null;

		public string PathOnDisk => FileDirectoryHelper.GetFullPath(GetType(), MetaData, false);

		private bool isDirty = false;

		/// <summary>
		/// True if the savefile has unsaved changes
		/// </summary>
		public bool IsDirty
		{
			get => isDirty;
			internal set
			{
				if (value)
				{
					OnDataChanged?.Invoke(); // Trying to set dirty to true implies there's been a change
				}

				if (value == isDirty)
				{
					return;
				}

				isDirty = value;

				if (value)
				{
					OnBecomeDirty?.Invoke();
				}
				else
				{
					OnBecomeClean?.Invoke();
				}
			}
		}

		/// <inheritdoc cref="Structs.MetaData"/>
		/// <seealso cref="SetMetaData"/>
		public MetaData MetaData { get; private set; }

		[SerializeField]
		private long creationTime = 0;

		public long RawCreationTime => creationTime;

		private DateTime creationDateTime = DateTime.UnixEpoch;

		public DateTime CreationTime
		{
			get
			{
				if (creationDateTime == DateTime.UnixEpoch)
				{
					creationDateTime = DateTime.FromBinary(creationTime);
				}

				return creationDateTime;
			}
			private set
			{
				creationDateTime = TimeZoneInfo.ConvertTimeToUtc(value);
				creationTime     = creationDateTime.ToBinary();
			}
		}

		[SerializeField]
		private long timeOfLastSave = 0;

		public long RawTimeOfLastSave => timeOfLastSave;
		public bool IsPresentOnDisk => timeOfLastSave != 0;

		private DateTime timeOfLastSaveDateTime = DateTime.UnixEpoch;

		public DateTime TimeOfLastSave
		{
			get
			{
				if (!IsPresentOnDisk)
				{
					return DateTime.UnixEpoch;
				}

				if (timeOfLastSaveDateTime == DateTime.UnixEpoch)
				{
					timeOfLastSaveDateTime = DateTime.FromBinary(timeOfLastSave);
				}

				return timeOfLastSaveDateTime;
			}
			internal set
			{
				timeOfLastSaveDateTime = TimeZoneInfo.ConvertTimeToUtc(value);
				timeOfLastSave         = timeOfLastSaveDateTime.ToBinary();
			}
		}

		protected AbstractSavableFile(MetaData metaData)
		{
			MetaData = metaData;
			
			// ReSharper disable once VirtualMemberCallInConstructor | Deliberate virtual call so we can encapsulate initialising all data (good for resetting as well)
			Initialise();
		}

		/// <summary>
		/// Invoked before the derived inspectors
		/// </summary>
		protected abstract void Initialise();
		
		/// <summary>
		/// Resets the values of this file to the default values
		/// </summary>
		/// <remarks>Does not reset <see cref="MetaData"/> or the <see cref="CreationTime"/></remarks>
		protected internal virtual void Reset()
		{
			SetDirty();
		}

		internal void SetAsNewFile()
		{
			Reset(); // Also sets IsDirty, because a new file has unsaved changes by definition

			creationTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now).ToBinary();

			SetAdditionalDataForNewFile();
		}

		/// <summary>
		/// Optionally, set additonal default data 
		/// </summary>
		protected internal abstract void SetAdditionalDataForNewFile();

		/// <summary>
		/// Set the creation time of this file
		/// </summary>
		/// <param name="creationTimeValue"></param>
		/// <remarks>Only to be used by parsers when loading a file, the creation time should never change after a file has been created</remarks>
		/// <seealso cref="SetAsNewFile"/>
		public void SetCreationTime(long creationTimeValue)
		{
			creationTime     = creationTimeValue;
			creationDateTime = DateTime.FromBinary(creationTime);
		}

		/// <summary>
		/// Sets this savefile to dirty
		/// </summary>
		/// <seealso cref="IsDirty"/>
		public void SetDirty()
		{
			IsDirty = true;
		}

		public void SetMetaData(MetaData metaData)
		{
			if (!MetaData.IsValid()) // The meta information was invalid, so this is the first time we set it (either because the file has just been parsed or was newly created)
			{
				MetaData = metaData; // Don't have to set IsDirty: a new file is already dirty and a freshly parsed file is never dirty.
				return;
			}

			metaData.ThrowIfInvalid();

			MetaData oldMetaData = MetaData;
			MetaData = metaData;

			FileParser.MoveFile(oldMetaData, this);
		}

		/// <summary>
		/// Returns a copy of this file with the given metadata
		/// </summary>
		/// <param name="metaData">The metadata of the new file</param>
		protected internal abstract AbstractSavableFile Duplicate(MetaData metaData);

		/// <summary>
		/// Two files are equal if their resolved path and creation time are equal
		/// </summary>
		/// <seealso cref="FileDirectoryHelper.GetFullPath"/>
		public bool Equals(AbstractSavableFile other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (!RawCreationTime.Equals(other.RawCreationTime))
			{
				return false;
			}

			return PathOnDisk.Equals(other.PathOnDisk, StringComparison.InvariantCulture);
		}

		/// <summary>
		/// Two files are equal if their resolved path and creation time are equal
		/// </summary>
		/// <seealso cref="FileDirectoryHelper.GetFullPath"/>
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

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((AbstractSavableFile)obj);
		}

		public override int GetHashCode()
		{
			return RawCreationTime.GetHashCode();
		}
	}
}