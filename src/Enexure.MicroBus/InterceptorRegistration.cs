using System;
using System.Collections.Generic;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class GlobalHandlerRegistration
	{
		public Type HandlerType { get; }
		public IEnumerable<Type> Dependencies { get; }

		public GlobalHandlerRegistration(Type delegatingHandlerType)
		{
			this.HandlerType = delegatingHandlerType;
			this.Dependencies = new Type[] { };

			Validate();
		}

		public GlobalHandlerRegistration(Type delegatingHandlerType, IEnumerable<Type> dependencies)
		{
			this.HandlerType = delegatingHandlerType;
			this.Dependencies = dependencies;

			Validate();
		}

		private void Validate()
		{
			if (!typeof(IDelegatingHandler).GetTypeInfo().IsAssignableFrom(HandlerType.GetTypeInfo())) {
				throw new TypeIsNotDelegatingHandlerException(HandlerType);
			}
		}
	}
}