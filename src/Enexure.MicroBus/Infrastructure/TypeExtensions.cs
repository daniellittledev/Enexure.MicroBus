using System;

namespace Enexure.MicroBus
{
	internal static class TypeExtensions
	{
		internal static bool IsInstantiable(this Type type)
		{
			return !type.IsInterface 
				|| !type.IsAbstract
				|| !type.IsGenericType;
		}
	}
}
