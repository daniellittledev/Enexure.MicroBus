using System.Reflection;

namespace Enexure.MicroBus
{
	internal static class TypeExtensions
	{
		internal static bool IsInstantiable(this TypeInfo type)
		{
			return !type.IsInterface 
				|| !type.IsAbstract
				|| !type.IsGenericType;
		}
	}
}
