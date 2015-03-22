using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface IQueryHandlerFinder
	{
		IReadOnlyCollection<Type> GetQueryHandlers();
	}
}
