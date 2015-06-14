using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IHandlerActivator
	{
		T ActivateHandler<T>(Type type, IPipelineHandler innerHandler);
		IEnumerable<T> ActivateHandlers<T>(MessageRegistration registration);
	}
}