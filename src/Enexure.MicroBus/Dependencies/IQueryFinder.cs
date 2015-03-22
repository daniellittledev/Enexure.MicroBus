using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Dependencies
{
	public interface IQueryFinder
	{
		IReadOnlyCollection<Type> GetQuerys();
	}
}
