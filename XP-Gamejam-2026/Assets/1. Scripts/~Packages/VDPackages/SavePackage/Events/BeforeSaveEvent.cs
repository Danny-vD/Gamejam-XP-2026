using System;
using VDFramework.EventSystem;
using VDPackages.SavePackage.SavableFiles.BaseClasses;

namespace VDPackages.SavePackage.Events
{
	/// <summary>
	/// Raised before the respective save is saved to give listeners the opportunity to write to it
	/// </summary>
	public class BeforeSaveEvent : VDEvent<BeforeSaveEvent>
	{
		public readonly Type FileType;
		public readonly AbstractSavableFile SavableFile;

		public BeforeSaveEvent(Type fileType, AbstractSavableFile savableFile)
		{
			FileType    = fileType;
			SavableFile = savableFile;
		}
	}
}