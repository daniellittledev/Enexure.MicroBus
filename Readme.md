Enexure.MicroBus
=================
[![Build status](https://ci.appveyor.com/api/projects/status/nwb1ebtfxiedyput/branch/master?svg=true)](https://ci.appveyor.com/project/Daniel45729/enexure-microbus/branch/master)

MicroBus is a simple in process mediator for .NET

> PM> Install-Package [Enexure.MicroBus](https://www.nuget.org/packages/Enexure.MicroBus/)

I wanted a super simple mediator with as few dependencies as possible. You can register handlers and pipelines without dependency injection, use a dependency injection framework or write your own activator. 

MicroBus supports the three fundamental bus message types, commands, events and queries(request/response). 

	bus.Send(new TestCommand());
	
	bus.Query(new TestQuery());
	
	bus.Publish(new TestEvent());
	
Here's what a command and it's handler look like
	
	class TestCommand : ICommand
	{
	}
	
	class TestCommandHandler : ICommandHandler<TestCommand>
	{
		public async Task Handle(TestCommand command)
		{
			Console.WriteLine("Test command handler");
		}
	}

If you need to handle cross cutting concerns you can use a pipeline handler

	public class CrossCuttingHandler : IPipelineHandler
	{
		public async Task<object> Handle(Func<IMessage, Task<object>> next, IMessage message)
		{
			Console.WriteLine("Cross cutting handler");
			return await next(message);
		}
	}
	
[Registration with Autofac](https://www.nuget.org/packages/Enexure.MicroBus.Autofac/) would look like this
	
	var containerBuilder = new ContainerBuilder();

	var pipline = new Pipeline()
		.AddHandler<CrossCuttingHandler>();

	containerBuilder.RegisterMicroBus(busBuilder => busBuilder
		.RegisterCommand<TestCommand>().To<TestCommandHandler>(pipline)
		.RegisterCommand<TestEvent>().To<TestEventHandler>(pipline)
	).Build();

	var container = containerBuilder.Build();

For more examples check out the [Enexure.MicroBus.Tests](https://github.com/Lavinski/Enexure.MicroBus/tree/master/src/Enexure.MicroBus.Tests) project.

	