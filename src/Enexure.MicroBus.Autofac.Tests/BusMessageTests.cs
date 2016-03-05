using Autofac;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.Autofac.Tests
{
	public class BusMessageTests
	{
		[Fact]
		public async Task FullSystemTest()
		{
			var assembly = GetType().GetTypeInfo().Assembly;
			var containerBuilder = new ContainerBuilder();
			var busBuilder = new BusBuilder()
				.RegisterInterceptor<OuterInterceptor>()
				.RegisterInterceptor<InnerInterceptor>()
				.RegisterHandlers(assembly, x => x.FullName.Contains("BusMessageTests"));

			containerBuilder.RegisterMicroBus(busBuilder);

			var container = containerBuilder.Build();

			var mediator = container.Resolve<IMicroMediator>();

			await mediator.SendAsync(new Message());
		}

		class Message
		{
			Queue<string> queue = new Queue<string>(new[] {
				"Outer-In",
				"Inner-In",
				"Handler",
				"Inner-Out",
				"Outer-Out",
			});

			public void AssertStage(string stageName)
			{
				Assert.Equal(queue.Dequeue(), stageName);
			}
		}

		class Handler : IMessageHandler<Message, Unit>
		{
			public Task<Unit> Handle(Message message)
			{
				message.AssertStage("Handler");

				return Task.FromResult(Unit.Unit);
			}
		}

		class OuterInterceptor : IInterceptor
		{
			public async Task<IReadOnlyCollection<object>> Handle(IInterceptorChain next, object message)
			{
				(message as Message).AssertStage("Outer-In");

				var result = await next.Handle(message);

				(message as Message).AssertStage("Outer-Out");

				return result;
			}
		}

		class InnerInterceptor : IInterceptor
		{
			public async Task<IReadOnlyCollection<object>> Handle(IInterceptorChain next, object message)
			{
				(message as Message).AssertStage("Inner-In");

				var result = await next.Handle(message);

				(message as Message).AssertStage("Inner-Out");

				return result;
			}
		}
	}
}
