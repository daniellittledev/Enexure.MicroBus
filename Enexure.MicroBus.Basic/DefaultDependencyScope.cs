using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	internal class DefaultDependencyScope : DefaultDependencyResolver, IDependencyScope
	{
		public object GetService(Type serviceType)
		{
			return Activator.CreateInstance(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			throw new NotSupportedException();
		}

		public T GetService<T>()
		{
			return (T)GetService(typeof(T));
		}

		public IEnumerable<T> GetServices<T>()
		{
			return (IEnumerable<T>)GetService(typeof(IEnumerable<T>));
		}

		public void Dispose()
		{
		}
	}
}