using System;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		private readonly Func<IMessageRegister, IMessageRegister> register;

		public BusBuilder(Func<IMessageRegister, IMessageRegister> register)
		{
			this.register = register;
		}

		public static IMicroBus BuildBus(Func<IMessageRegister, IMessageRegister> register)
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
			var handlerProvider = new HandlerProvider(registrations);
			var pipelineBuilder = new PipelineBuilder(handlerProvider, busSettings);
			var dependencyResolver = new DefaultDependencyResolver();

			return new MicroBus(pipelineBuilder, dependencyResolver);
		}
	}
}