using Autofac;

namespace Enexure.MicroBus.Autofac
{
	internal class AutofacDependencyResolver : IDependencyResolver
	{
		protected readonly ILifetimeScope lifetimeScope;

		public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
		{
			this.lifetimeScope = lifetimeScope;
		}

		public IDependencyScope BeginScope()
		{
			return new AutofacDependencyScope(lifetimeScope.BeginLifetimeScope());
		}
	}
}