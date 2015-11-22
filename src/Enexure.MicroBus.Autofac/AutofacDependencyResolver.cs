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
			var scope = (lifetimeScope.Tag as string == "MicroBus") 
				? lifetimeScope 
				: lifetimeScope.BeginLifetimeScope("MicroBus");

			return new AutofacDependencyScope(scope);
		}
	}
}