using Autofac;
using Autofac.Builder;

namespace Sample.Autofac
{
	public static class AutofacExtensions
	{
		public static IRegistrationBuilder<THandler, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterCommandHandler<THandler>(this ContainerBuilder containerBuilder)
		{
			return containerBuilder.RegisterType<THandler>();
		}
	}
}