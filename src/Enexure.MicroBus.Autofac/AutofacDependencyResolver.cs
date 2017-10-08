using Autofac;

namespace Enexure.MicroBus.Autofac
{
    internal class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope lifetimeScope;

        public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IDependencyScope BeginScope()
        {
            var isNested = lifetimeScope.Tag as string == "MicroBus";
            var scope = isNested ? lifetimeScope : lifetimeScope.BeginLifetimeScope("MicroBus");

            return new AutofacDependencyScope(scope, !isNested);
        }
    }
}