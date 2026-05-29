using System;

namespace VDPackages.SavePackage.Parsing.Utility
{
	public static class TypeFromStringHelper
	{
		public static bool TryGetType(string typeString, out Type type)
		{
			if (string.IsNullOrEmpty(typeString))
			{
				type = default;
				return false;
			}

			type = Type.GetType(typeString, false);

			return type != null;
		}
	}
}