using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface ICommandFinder
	{
		IReadOnlyCollection<Type> GetCommands();
	}
}
