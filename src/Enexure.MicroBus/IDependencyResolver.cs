namespace Enexure.MicroBus
{
    public interface IDependencyResolver
    {
        IDependencyScope BeginScope();
    }
}