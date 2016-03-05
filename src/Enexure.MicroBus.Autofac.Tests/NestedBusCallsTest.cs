using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
	public class NestedBusCallsTest
	{
		interface IPipeTestMessage
		{
			List<Guid> HandlerIds { get; set; }
		}

		class Command : ICommand, IPipeTestMessage
		{
			public Command()
			{
				HandlerIds = new List<Guid>();
			}

			public bool Run { get; set; }

			public List<Guid> HandlerIds { get; set; }

		}

		[UsedImplicitly]
		class PipelineHandler : IPipelineHandler
		{
			private readonly Guid id;

			public PipelineHandler()
			{
				id = Guid.NewGuid();
			}

			public Task<object> Handle(Func<IMessage, Task<object>> next, IMessage message)
			{
				var pipeMessage = message as IPipeTestMessage;
				if (pipeMessage != null)
				{
					pipeMessage.HandlerIds.Add(id);
				}

				return next(message);
			}
		}

		[UsedImplicitly]
		class CommandHandler : ICommandHandler<Command>
		{
			private readonly IMicroBus bus;

			public CommandHandler(IMicroBus bus)
			{
				this.bus = bus;
			}

			public async Task Handle(Command command)
			{
				var @event = new Event();
				await bus.PublishAsync(@event);
				command.Run = @event.Run;
				command.HandlerIds.AddRange(@event.HandlerIds);
			}

		}

		class Event : IEvent, IPipeTestMessage
		{
			public Event()
			{
				HandlerIds = new List<Guid>();
			}

			public bool Run { get; set; }

			public List<Guid> HandlerIds { get; set; }

		}

		[UsedImplicitly]
		class EventHandler : IEventHandler<Event>
		{
			public Task Handle(Event @event)
			{
				@event.Run = true;
				return Task.FromResult(0);
			}
		}

		[Fact]
		public async Task SendingACommandThatRaisesAnEventShouldNotThrow()
		{
			var busBuilder = new BusBuilder()
				.RegisterCommandHandler<Command, CommandHandler>()
				.RegisterEventHandler<Event, EventHandler>();

			var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

			var bus = container.Resolve<IMicroBus>();

			var command = new Command();
			await bus.SendAsync(command);

			command.Run.Should().Be(true);
		}

		[UsedImplicitly]
		class GlobalPipelineHandler : IPipelineHandler
		{
			readonly IOuterPipelineDetector detector;

			public GlobalPipelineHandler(IOuterPipelineDetector detector)
			{
				this.detector = detector;
			}

			public Task<object> Handle(Func<IMessage, Task<object>> next, IMessage message)
			{
				if (detector.IsOuterPipeline)
				{
					var pipeMessage = message as IPipeTestMessage;
					if (pipeMessage != null)
					{
						pipeMessage.HandlerIds.Add(Guid.NewGuid());
					}
				}
				return next(message);
			}
		}

		[Fact]
		public async Task GlobalPipelineHandlerShouldOnlyBeRunOnce()
		{
			var busBuilder = new BusBuilder()
				.RegisterCommandHandler<Command, CommandHandler>()
				.RegisterEventHandler<Event, EventHandler>()
				.RegisterPipelineHandler<GlobalPipelineHandler>();

			var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

			var bus = container.Resolve<IMicroBus>();

			var command = new Command();
			await bus.SendAsync(command);

			command.Run.Should().Be(true);

			command.HandlerIds.Distinct().Should().HaveCount(1, "Global pipeline handler should only be run once");
		}

		[Fact]
		public async Task GlobalPipelineHandlerShouldBeRunOnce()
		{
			var busBuilder = new BusBuilder()
				.RegisterEventHandler<Event, EventHandler>()
				.RegisterPipelineHandler<GlobalPipelineHandler>();

			var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

			var bus = container.Resolve<IMicroBus>();

			var evt = new Event();
			await bus.PublishAsync(evt);

			evt.Run.Should().Be(true);

			evt.HandlerIds.Distinct().Should().HaveCount(1, "Global pipeline handler should be run");
		}
	}
}
