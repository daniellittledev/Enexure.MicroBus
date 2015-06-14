using System;
using System.Collections.Generic;
using System.Linq;
using Enexure.MicroBus;

namespace Enexure.MicroBus
{
	public class DefaultHandlerActivator : IHandlerActivator
	{
		public T ActivateHandler<T>(Type type, IPipelineHandler innerHandler)
		{
			return (T)Activator.CreateInstance(type, innerHandler);
		}

		public IEnumerable<T> ActivateHandlers<T>(MessageRegistration registration)
		{
			return registration.Handlers.Select(handlerType => (T)Activator.CreateInstance(handlerType));
		}
	}
}