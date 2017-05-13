using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Enexure.MicroBus.MicrosoftDependencyInjection
{
    internal class MicrosoftDependencyInjectionDependencyScope : MicrosoftDependencyInjectionDependencyResolver, IDependencyScope
    {
        private readonly IServiceProvider serviceProvider;

        public MicrosoftDependencyInjectionDependencyScope(IServiceProvider serviceProvider, IMarker marker)
            : base(serviceProvider, marker)
        {
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceProvider.GetServices(serviceType);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public IEnumerable<T> GetServices<T>()
        {
            return GetServices(typeof(T)).Cast<T>();
        }

        public void Dispose()
        {
        }
    }
}