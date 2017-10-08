using System;
using System.Threading;
using System.Threading.Tasks;
using Enexure.MicroBus.Messages;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.UnitTests.PipelineBuilderTests
{
    public class NoHandlerRegistrationTests
    {
        class TestCommand : ICommand
        { }

        [Fact]
        public void NoMatchingRegistrationWithNoNoMatchingRegistrationEventHandler()
        {
            var busSettings = new BusSettings();
            var updater = new OuterPipelineDetector();
            var dependencyScope = new TestDependencyScope();

            var busBuilder = new BusBuilder();
            var pipelineBuilder = new PipelineBuilder(busBuilder);
            var pipelineRunBuilder = new PipelineRunBuilder(busSettings, pipelineBuilder,updater, dependencyScope);

            Action action = () => pipelineRunBuilder.GetRunnerForPipeline(typeof(TestCommand), default(CancellationToken));

            action.ShouldThrowExactly<NoRegistrationForMessageException>().And.MessageType.Should().Be(typeof(TestCommand));
        }

        [Fact]
        public async Task HandlingCommandWithNoRegisteredHandlerShouldRunNoMatchingRegistrationHandler() {

            var busSettings = new BusSettings();
            var updater = new OuterPipelineDetector();
            var dependencyScope = new TestDependencyScope();

            var handler = new TestEventHandler();
            var handlerShim = new EventHandlerShim<NoMatchingRegistrationEvent, TestEventHandler>(handler);
            dependencyScope.AddObject(handlerShim);

            var busBuilder = new BusBuilder()
                .RegisterEventHandler<NoMatchingRegistrationEvent, TestEventHandler>();
            var pipelineBuilder = new PipelineBuilder(busBuilder);
            var pipelineRunBuilder = new PipelineRunBuilder(busSettings, pipelineBuilder, updater, dependencyScope);

            var runner = pipelineRunBuilder.GetRunnerForPipeline(typeof(TestCommand), default(CancellationToken));

            await runner.Handle(new TestCommand());

            handler.CallsToHandle.Should().Be(1);
        }

        class TestEventHandler : IEventHandler<NoMatchingRegistrationEvent>
        {
            public int CallsToHandle { get; set; }

            public Task Handle(NoMatchingRegistrationEvent @event)
            {
                CallsToHandle += 1;

                return Task.FromResult(1);
            }
        }
    }
}
