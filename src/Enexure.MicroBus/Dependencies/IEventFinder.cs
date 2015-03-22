using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface IEventFinder
	{
		IReadOnlyCollection<Type> GetEvents();
	}
}
