using System;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		private readonly Func<IHandlerRegister, IHandlerRegister> register;
		private readonly Pipeline pipeline;

		private BusBuilder(Func<IHandlerRegister, IHandlerRegister> register, Pipeline pipeline)
		{
			this.register = register;
			this.pipeline = pipeline;
		}

		public static IMicroBus BuildBus(Func<IHandlerRegister, IHandlerRegister> register)
		{
			return new BusBuilder(register, Pipeline.EmptyPipeline).BuildBus();
		}

		public static IMicroBus BuildBus(Func<IHandlerRegister, IHandlerRegister> register, Pipeline pipeline)
		{
			return new BusBuilder(register, pipeline).BuildBus();
		}

		public IMicroBus BuildBus()
		{
			return BuildBus(new BusSettings());
		}

		public IMicroBus BuildBus(BusSettings busSettings)
		{
			var register = this.register(new HandlerRegister());
			var registrations = register.GetMessageRegistrations();
			var handlerProvider = HandlerProvider.Create(registrations);

			var globalPipelineProvider = new GlobalPipelineProvider(pipeline);
			var pipelineBuilder = new PipelineBuilder(handlerProvider, globalPipelineProvider, busSettings);
			var dependencyResolver = new DefaultDependencyResolver();

			return new MicroBus(pipelineBuilder, dependencyResolver);
		}
	}
}