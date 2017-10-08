using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
    internal class AutofacDependencyScope : AutofacDependencyResolver, IDependencyScope
    {
        private readonly bool ownsLifetimeScope;
        private readonly ILifetimeScope lifetimeScope;

        public AutofacDependencyScope(ILifetimeScope lifetimeScope)
                    : base(lifetimeScope)
        {
            this.ownsLifetimeScope = false;
            this.lifetimeScope = lifetimeScope;
        }

        public AutofacDependencyScope(ILifetimeScope lifetimeScope, bool ownsLifetimeScope)
            : base(lifetimeScope)
        {
            this.ownsLifetimeScope = ownsLifetimeScope;
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
            if (this.ownsLifetimeScope)
            {
                lifetimeScope.Dispose();
            }
        }
    }
}