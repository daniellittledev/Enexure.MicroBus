using System;
using System.Collections.Generic;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class InterceptorRegistration
	{
		public Type InterceptorType { get; }
		public IEnumerable<Type> Dependencies { get; }

		public InterceptorRegistration(Type interceptorType)
		{
			this.InterceptorType = interceptorType;
			this.Dependencies = new Type[] { };

			Validate();
		}

		public InterceptorRegistration(Type interceptorType, IEnumerable<Type> dependencies)
		{
			this.InterceptorType = interceptorType;
			this.Dependencies = dependencies;

			Validate();
		}

		private void Validate()
		{
			if (!typeof(IInterceptor).GetTypeInfo().IsAssignableFrom(InterceptorType.GetTypeInfo())) {
				throw new TypeIsNotAnInterceptorException(InterceptorType);
			}
		}
	}
}