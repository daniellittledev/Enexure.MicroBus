using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface ICommandHandlerFinder
	{
		IReadOnlyCollection<Type> GetCommandHandlers();
	}
}
