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

		private IMicroBus BuildBus()
		{
			return BuildBus(new BusSettings());
		}

		private IMicroBus BuildBus(BusSettings busSettings)
		{
			var tracker = new GlobalPipelineTracker();

			var register = this.register(new HandlerRegister());
			var registrations = register.GetMessageRegistrations();
			var handlerProvider = HandlerProvider.Create(registrations);

			throw new NotImplementedException();

			//var globalPipelineProvider = new GlobalPipelineProvider(pipeline);
			//var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, tracker);

			var dependencyResolver = new DefaultDependencyResolver();

			return new MicroBus(dependencyResolver);
		}
	}
}