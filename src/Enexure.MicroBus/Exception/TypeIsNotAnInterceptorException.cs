using System;

namespace Enexure.MicroBus
{
	internal class TypeIsNotAnInterceptorException : Exception
	{
		public TypeIsNotAnInterceptorException(Type type)
			: base ($"The type {type.FullName} is not an interceptor")
		{
		}
	}
}