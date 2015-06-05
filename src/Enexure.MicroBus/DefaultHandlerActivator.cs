using System;
using Enexure.MicroBus.InfrastructureContracts;

namespace Enexure.MicroBus
{
	public class DefaultHandlerActivator : IHandlerActivator
	{
		public T ActivateHandler<T>(Type type)
		{
			return (T)Activator.CreateInstance(type);
		}

		public T ActivateHandler<T>(Type type, IPipelineHandler innerHandler)
		{
			return (T)Activator.CreateInstance(type, innerHandler);
		}
	}
}