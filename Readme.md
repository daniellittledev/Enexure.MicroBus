Enexure.MicroBus
=================
[![Build status](https://ci.appveyor.com/api/projects/status/nwb1ebtfxiedyput/branch/master?svg=true)](https://ci.appveyor.com/project/Daniel45729/enexure-microbus/branch/master)

MicroBus is a simple in process mediator for .NET

Nuget package not yet building.
<s>
> PM> Install-Package [Enexure.MicroBus](https://www.nuget.org/packages/Enexure.MicroBus/)
</s>

Sample

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