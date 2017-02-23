namespace Enexure.MicroBus.Tests.IntegrationTests.Cancellation
{
	using System.Threading.Tasks;

	using Enexure.MicroBus.Autofac;

	using global::Autofac;

	using Xunit;

	public class Issue5Tests
	{

		[Fact]
		public async Task TestCaseShouldNotThrowObjectDisposedException()
		{
			var builder = new ContainerBuilder();
			var busBuilder =
				new BusBuilder()
					.RegisterCommandHandler<TestCommand, TestCommandHandler>()
					.RegisterCommandHandler<TestNestedCommand, TestNestedCommandHandler>()
					.RegisterEventHandler<TestEvent, TestEventHandler>()
					.RegisterEventHandler<TestEvent, TestOtherEventHandler>();
			builder.RegisterMicroBus(busBuilder, new BusSettings() { HandlerSynchronization = Synchronization.Syncronous });

			using (var container = builder.Build())
			{
				var bus = container.Resolve<IMicroBus>();
				await bus.SendAsync(new TestCommand());
			}
		}

		private class TestCommand : ICommand { }

		private class TestEvent : IEvent { }

		private class TestNestedCommand : ICommand { }

		private class TestCommandHandler : ICommandHandler<TestCommand>
		{
			private readonly IMicroBus bus;

			public TestCommandHandler(IMicroBus bus)
			{
				this.bus = bus;
			}

			public async Task Handle(TestCommand command)
			{
				await this.bus.PublishAsync(new TestEvent());
			}
		}

		private class TestEventHandler : IEventHandler<TestEvent>
		{
			private readonly IMicroBus bus;

			public TestEventHandler(IMicroBus bus)
			{
				this.bus = bus;
			}

			public async Task Handle(TestEvent @event)
			{
				await this.bus.SendAsync(new TestNestedCommand());
			}
		}

		private class TestOtherEventHandler : IEventHandler<TestEvent>
		{
			private readonly IMicroBus bus;

			public TestOtherEventHandler(IMicroBus bus)
			{
				this.bus = bus;
			}

			public async Task Handle(TestEvent @event)
			{
				await Task.CompletedTask;
			}
		}

		private class TestNestedCommandHandler : ICommandHandler<TestNestedCommand>
		{
			private readonly IMicroBus bus;

			public TestNestedCommandHandler(IMicroBus bus)
			{
				this.bus = bus;
			}

			public async Task Handle(TestNestedCommand command)
			{
				await Task.CompletedTask;
			}
		}
	}
}
