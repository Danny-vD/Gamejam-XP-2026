using System;
using UnityEngine;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SavableFiles.Structs.DataStructs;

namespace VDPackages.SavePackage.SavableFiles.SpecificClasses
{
	[Serializable]
	public class GenericSaveFile : AbstractSavableFile
	{
		[SerializeField]
		private double playTimeSeconds = 0;

		public double PlayTimeSeconds => playTimeSeconds;

		public TimeSpan PlayTime
		{
			get => TimeSpan.FromSeconds(playTimeSeconds);
			set
			{
				playTimeSeconds = value.TotalSeconds;
				SetDirty();
			}
		}

		/// <summary>
		/// The name of this savefile as shown in-game
		/// </summary>
		/// <remarks>The name of the savefile itself (if present) is set in <see cref="MetaData.FileName"/></remarks>
		/// <seealso cref="MetaData"/>
		/// <seealso cref="SetDisplayName"/>
		[SerializeField]
		public string DisplayName;

		/// <summary>
		/// Data specific to the game
		/// </summary>
		/// <seealso cref="SetGameData"/>
		[SerializeField]
		public GameData GameData;

		public GenericSaveFile() : this(new MetaData()) // Invoked by parsers when deserializing (e.g. JsonUtility in Unity)
		{
		}

		public GenericSaveFile(MetaData metaData) : base(metaData)
		{
		}

		public GenericSaveFile(GenericSaveFile copyFrom, MetaData metaData) : base(metaData)
		{
			DisplayName = copyFrom.DisplayName;
			GameData    = copyFrom.GameData;
		}

		protected internal override AbstractSavableFile Duplicate(MetaData metaData)
		{
			return new GenericSaveFile(this, metaData);
		}

		protected override void Initialise()
		{
			GameData = GameData.GetDefault(this);
		}

		protected internal override void Reset()
		{
			base.Reset();

			Initialise();
			playTimeSeconds = 0;
		}

		protected internal override void SetAdditionalDataForNewFile()
		{
			DisplayName = Application.productName;
		}

		/// <summary>
		/// Set the <see cref="DisplayName"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetDisplayName(string newName)
		{
			DisplayName = newName;
			SetDirty();
		}

		/// <summary>
		/// Set the <see cref="GameData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetGameData(GameData gameData)
		{
			GameData = gameData;
			SetDirty();
		}

		public void AddPlayTime(double seconds)
		{
			if (seconds < 0)
			{
				return;
			}

			playTimeSeconds += seconds;
			SetDirty();
		}
	}
}