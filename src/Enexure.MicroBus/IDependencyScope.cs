using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IDependencyScope : IDependencyResolver, IDisposable
	{
		object GetService(Type serviceType);

		IEnumerable<object> GetServices(Type serviceType);

		T GetService<T>();

		IEnumerable<T> GetServices<T>();
	}
}