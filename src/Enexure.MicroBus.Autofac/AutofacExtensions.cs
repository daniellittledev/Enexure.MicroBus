using Autofac;
using Autofac.Builder;

namespace Enexure.MicroBus.Autofac
{
	public static class AutofacExtensions
	{
		public static IRegistrationBuilder<THandler, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterCommandHandler<THandler>(this ContainerBuilder containerBuilder)
		{
			return containerBuilder.RegisterType<THandler>();
		}
	}
}