using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	internal class AutofacDependencyScope : AutofacDependencyResolver, IDependencyScope
	{
		private readonly ILifetimeScope lifetimeScope;

		public AutofacDependencyScope(ILifetimeScope lifetimeScope)
			: base(lifetimeScope)
		{
			this.lifetimeScope = lifetimeScope;
		}

		public object GetService(Type serviceType)
		{
			return lifetimeScope.Resolve(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return ((IEnumerable)lifetimeScope.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType))).Cast<object>();
		}

		public void Dispose()
		{
			lifetimeScope.Dispose();
		}
	}
}