using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.MessageContracts;

namespace Sample.Autofac
{
	class Program
	{
		static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterMicroBus(builder => {

				var pipeline = builder.CreatePipeline()
					.AddHandler<CrossCuttingHandler>();

				builder.RegisterHandler<TestCommandHandler>(pipeline);
			});

			var container = containerBuilder.Build();

			container.Resolve<IBus>().Send(new TestCommand());

			Console.ReadLine();
		}
	}

	public class CrossCuttingHandler : IPipelineHandler
	{
		private readonly IPipelineHandler innerHandler;

		public CrossCuttingHandler(IPipelineHandler innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public async Task Handle(IMessage message)
		{
			Console.WriteLine("Cross cutting handler");

			await innerHandler.Handle(message);
		}
	}

	class TestCommandHandler : ICommandHandler<TestCommand>
	{
		public async Task Handle(TestCommand command)
		{
			Console.WriteLine("Test command handler");
		}
	}

	class TestCommand : ICommand
	{
	}
}
