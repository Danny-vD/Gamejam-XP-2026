using System;
using UnityEngine;
using VDPackages.SavePackage.SavableFiles.BaseClasses;
using VDPackages.SavePackage.SavableFiles.Structs;
using VDPackages.SavePackage.SavableFiles.Structs.DataStructs;
using VDPackages.SavePackage.SavableFiles.Structs.DataStructs.Settings;

namespace VDPackages.SavePackage.SavableFiles.SpecificClasses
{
	[Serializable]
	public class GenericConfigFile : AbstractSavableFile
	{
		/// <summary>
		/// Display Settings
		/// </summary>
		[SerializeField]
		public DisplaySettingsData DisplaySettingsData;
		
		/// <summary>
		/// Input Settings
		/// </summary>
		/// <seealso cref="SetInputSettings"/>
		[SerializeField]
		public InputSettingsData InputSettingsData;

		/// <summary>
		/// Audio Settings
		/// </summary>
		/// <seealso cref="SetAudioSettings"/>
		[SerializeField]
		public AudioSettingsData AudioSettingsData;

		/// <summary>
		/// Language Settings
		/// </summary>
		/// <seealso cref="SetLanguageSettings"/>
		[SerializeField]
		public LanguageSettingsData LanguageSettingsData;
		
		/// <summary>
		/// Gameplay Settings
		/// </summary>
		/// <seealso cref="SetGameplaySettings"/>
		[SerializeField]
		public GameplaySettingsData GameplaySettingsData;

		/// <summary>
		/// Settings specific to the game
		/// </summary>
		/// <seealso cref="SetGameSettings"/>
		[SerializeField]
		public GameData GameSettingsData;

		public GenericConfigFile() : this(new MetaData()) // Invoked by parsers when deserializing (e.g. JsonUtility in Unity)
		{
		}

		public GenericConfigFile(MetaData metaData) : base(metaData)
		{
		}

		public GenericConfigFile(GenericConfigFile copyFrom, MetaData metaData) : base(metaData)
		{
			DisplaySettingsData  = copyFrom.DisplaySettingsData;
			InputSettingsData    = copyFrom.InputSettingsData;
			AudioSettingsData    = copyFrom.AudioSettingsData;
			LanguageSettingsData = copyFrom.LanguageSettingsData;
			GameplaySettingsData = copyFrom.GameplaySettingsData;

			GameSettingsData = copyFrom.GameSettingsData;
		}
		
		protected internal override AbstractSavableFile Duplicate(MetaData metaData)
		{
			return new GenericConfigFile(this, metaData);
		}
		
		protected override void Initialise()
		{
			DisplaySettingsData  = DisplaySettingsData.GetDefault(this);
			InputSettingsData    = InputSettingsData.GetDefault(this);
			AudioSettingsData    = AudioSettingsData.GetDefault(this);
			LanguageSettingsData = LanguageSettingsData.GetDefault(this);
			GameplaySettingsData = GameplaySettingsData.GetDefault(this);

			GameSettingsData = GameData.GetDefault(this);
		}

		protected internal override void Reset()
		{
			Initialise();
			
			base.Reset();
		}

		protected internal override void SetAdditionalDataForNewFile()
		{
		}

		/// <summary>
		/// Set the <see cref="InputSettingsData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetInputSettings(InputSettingsData inputSettingsData)
		{
			InputSettingsData = inputSettingsData;
			SetDirty();
		}

		/// <summary>
		/// Set the <see cref="AudioSettingsData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetAudioSettings(AudioSettingsData audioSettingsData)
		{
			AudioSettingsData = audioSettingsData;
			SetDirty();
		}

		/// <summary>
		/// Set the <see cref="LanguageSettingsData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetLanguageSettings(LanguageSettingsData languageSettingsData)
		{
			LanguageSettingsData = languageSettingsData;
			SetDirty();
		}
		
		/// <summary>
		/// Set the <see cref="GameplaySettingsData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetGameplaySettings(GameplaySettingsData gameplaySettingsData)
		{
			GameplaySettingsData = gameplaySettingsData;
			SetDirty();
		}

		/// <summary>
		/// Set the <see cref="GameSettingsData"/> to the given value and marks the file as dirty
		/// </summary>
		/// <seealso cref="AbstractSavableFile.IsDirty"/>
		public void SetGameSettings(GameData gameSettingsData)
		{
			GameSettingsData = gameSettingsData;
			SetDirty();
		}
	}
}