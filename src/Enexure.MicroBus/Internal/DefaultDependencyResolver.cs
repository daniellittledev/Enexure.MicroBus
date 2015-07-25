namespace Enexure.MicroBus
{
	internal class DefaultDependencyResolver : IDependencyResolver
	{
		public IDependencyScope BeginScope()
		{
			return new DefaultDependencyScope();
		}
	}
}