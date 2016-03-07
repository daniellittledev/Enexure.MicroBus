using System;

namespace Enexure.MicroBus
{
	internal class TypeIsNotDelegatingHandlerException : Exception
	{
		public TypeIsNotDelegatingHandlerException(Type type)
			: base ($"The type {type.FullName} is not an interceptor")
		{
		}
	}
}