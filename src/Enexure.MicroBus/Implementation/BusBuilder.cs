using System;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		private readonly Func<IHandlerRegister, IHandlerRegister> register;

		public BusBuilder(Func<IHandlerRegister, IHandlerRegister> register)
		{
			this.register = register;
		}

		public static IMicroBus BuildBus(Func<IHandlerRegister, IHandlerRegister> register)
		{
			return new BusBuilder(register).BuildBus();
		}

		public IMicroBus BuildBus()
		{
			return BuildBus(new BusSettings());
		}

		public IMicroBus BuildBus(BusSettings busSettings)
		{
			var registrations = register(new HandlerRegister()).GetMessageRegistrations();
			var handlerProvider = HandlerProvider.Create(registrations);
			var pipelineBuilder = new PipelineBuilder(handlerProvider, busSettings);
			var dependencyResolver = new DefaultDependencyResolver();

			return new MicroBus(pipelineBuilder, dependencyResolver);
		}
	}
}