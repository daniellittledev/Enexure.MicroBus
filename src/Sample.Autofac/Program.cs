using System;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus;
using Enexure.MicroBus.MessageContracts;

namespace Sample.Autofac
{
	class Program
	{
		static void Main(string[] args)
		{
			var containerBuilder = new ContainerBuilder();

				var builder = new BusBuilder();


				var pipeline = new Pipeline()
					.AddHandler<CrossCuttingHandler>();

				builder.Register<TestCommandHandler>(pipeline);

				var bus = builder.BuildBus();


			var container = containerBuilder.Build();


			bus.Send(new TestCommand());

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
