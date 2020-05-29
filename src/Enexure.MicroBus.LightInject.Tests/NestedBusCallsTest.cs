using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightInject;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.LightInject.Tests
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

        class PipelineHandler : IDelegatingHandler
        {
            private readonly Guid id;

            public PipelineHandler()
            {
                id = Guid.NewGuid();
            }

            public Task<object> Handle(INextHandler next, object message)
            {
                if (message is IPipeTestMessage pipeMessage)
                {
                    pipeMessage.HandlerIds.Add(id);
                }

                return next.Handle(message);
            }
        }

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

            var container = new ServiceContainer();
            container.RegisterMicroBus(busBuilder);

            var bus = container.GetInstance<IMicroBus>();

            var command = new Command();
            await bus.SendAsync(command);

            command.Run.Should().Be(true);
        }

        class GlobalDelegatingHandler : IDelegatingHandler
        {
            readonly IOuterPipelineDetector detector;

            public GlobalDelegatingHandler(IOuterPipelineDetector detector)
            {
                this.detector = detector;
            }

            public Task<object> Handle(INextHandler next, object message)
            {
                if (detector.IsOuterPipeline)
                {
                    var pipeMessage = message as IPipeTestMessage;
                    pipeMessage?.HandlerIds.Add(Guid.NewGuid());
                }
                return next.Handle(message);
            }
        }

        [Fact]
        public async Task GlobalDelegatingHandlerShouldOnlyBeRunOnce()
        {
            var busBuilder = new BusBuilder()
                .RegisterCommandHandler<Command, CommandHandler>()
                .RegisterEventHandler<Event, EventHandler>()
                .RegisterGlobalHandler<GlobalDelegatingHandler>();

            var container = new ServiceContainer();
            container.RegisterMicroBus(busBuilder);

            var bus = container.GetInstance<IMicroBus>();

            var command = new Command();
            await bus.SendAsync(command);

            command.Run.Should().Be(true);

            command.HandlerIds.Distinct().Should().HaveCount(1, "Global pipeline handler should only be run once");
        }

        [Fact]
        public async Task GlobalDelegatingHandlerShouldBeRunOnce()
        {
            var busBuilder = new BusBuilder()
                .RegisterEventHandler<Event, EventHandler>()
                .RegisterGlobalHandler<GlobalDelegatingHandler>();

            var container = new ServiceContainer();
            container.RegisterMicroBus(busBuilder);

            var bus = container.GetInstance<IMicroBus>();

            var evt = new Event();
            await bus.PublishAsync(evt);

            evt.Run.Should().Be(true);

            evt.HandlerIds.Distinct().Should().HaveCount(1, "Global pipeline handler should be run");
        }
    }
}
