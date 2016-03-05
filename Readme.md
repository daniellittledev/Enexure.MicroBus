Enexure.MicroBus
=================
[![Build status](https://ci.appveyor.com/api/projects/status/nwb1ebtfxiedyput/branch/master?svg=true)](https://ci.appveyor.com/project/Daniel45729/enexure-microbus/branch/master)

MicroBus is a simple in process mediator for .NET

> PM> Install-Package [Enexure.MicroBus](https://www.nuget.org/packages/Enexure.MicroBus/)

I wanted a super simple mediator with great interceptor support. With MicroBus  handlers and interceptors are first class citizens making it easy to get started. 

    var busBuilder = new BusBuilder()
        .RegisterInterceptor<LoggingInterceptor>()
        .RegisterInterceptor<SecurityInterceptor>()
        .RegisterInterceptor<ValidationInterceptor>()
        .RegisterInterceptor<TransactionInterceptor>()

        .RegisterHandlers(assembly)        

	    .RegisterCommandHandler<Command, CommandHandler>()
	    .RegisterEventHandler<Event, EventHandler>()
	    .RegisterMessageHandler<Message, MessageHandler>();

After your registrations are sorted out then it's just a matter of registering MicroBus with a DI container. Here is now we register MicroBus to autofac.

    autofacContainerBuilder.RegisterMicroBus(busBuilder);

MicroBus has two main interfaces, the bus and the mediator. A bus will work with the message types, commands, events and queries(request/response). This imposes a strong set of rules around what a message can do. For example you can only have at most one handler for a query or command.

    given(IMicroBus bus)
    
	bus.SendAsync(new TestCommand());  // ICommand	
	bus.QueryAsync(new TestQuery());   // IQuery<Query, Result>
	bus.PublishAsync(new TestEvent()); // IEvent

The other is the mediator which is more general. Messages can be anything and don't need to implement any specific interface. This can be useful when interfacing with existing message contracts.

     given(IMicroMediator mediator)

     mediator.SendAsync(anyObject);
     mediator.QueryAsync(anyObject);

Commands are the typical entry point for an application. A command is something that you ask the system to do. For example, create a new page the the name of a command is always in the imperative tense. Commands are also interesting in that they don't return anything. In our create a page example you would say create the page "home" or create an object that I can refer to with this Guid instead of create a page and return the Id. The great part about this is you already know what the Id of the resource is going to be.
	
	class TestCommand : ICommand { }
	
	class TestCommandHandler : ICommandHandler<TestCommand>
	{
		public async Task Handle(TestCommand command)
		{
			Console.WriteLine("Test command handler");
		}
	}

One of the most important things an application needs is a way to deal with cross cutting concerns. In MicroBus this is where interceptors come in. They provide a great way to do work before messages get to the handlers. They also use the nested Russian Doll style approach so each interceptor will internally call the next interceptor or handler in the chain. 

	public class CrossCuttingInterceptor : IInterceptor
	{
		public async Task<IReadOnlyCollection<object>> Handle(IInterceptorChain next, IMessage message)
		{
			using (var transaction = unitOfWork.NewTransaction()) {
			    return await next.Handle(message);
			}
		}
	}
	
Currently MicroBus only comes with built in [support for Autofac](https://www.nuget.org/packages/Enexure.MicroBus.Autofac/).

For more examples check out the [Enexure.MicroBus.Tests](https://github.com/Lavinski/Enexure.MicroBus/tree/master/src/Enexure.MicroBus.Tests) project.

	