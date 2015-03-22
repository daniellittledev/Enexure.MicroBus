using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface IEventHandlerFinder
	{
		IReadOnlyCollection<Type> GetEventHandlers();
	}
}
