using System;

namespace Enexure.MicroBus.InfrastructureContracts
{
	public interface IHandlerActivator
	{
		T ActivateHandler<T>(Type type);
		T ActivateHandler<T>(Type type, IPipelineHandler innerHandler);
	}
}