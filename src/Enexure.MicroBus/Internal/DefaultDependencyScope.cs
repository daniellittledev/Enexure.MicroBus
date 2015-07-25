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
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}
	}
}