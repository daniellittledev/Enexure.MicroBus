using Enexure.MicroBus.Messages;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Enexure.MicroBus.Tests.PipelineBuilderTests
{
	[TestFixture]
	public class NoMessageRegistrationTests
	{
		class TestCommand : ICommand
		{ }

		[Test]
		public void NoMatchingRegistrationWithNoNoMatchingRegistrationEventHandler()
		{
			var busSettings = new BusSettings();
			var globalPipelineProvider = Substitute.For<IGlobalPipelineProvider>();
			var globalPipelineTracker = Substitute.For<IGlobalPipelineTracker>();
			var dependencyScope = Substitute.For<IDependencyScope>();

			var handlerProvider = HandlerProvider.Create(new MessageRegistration[] {});

			globalPipelineProvider.GetGlobalPipeline().Returns(Pipeline.EmptyPipeline);

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			Action action = () => pipelineBuilder.GetPipelineForMessage(typeof(TestCommand));

			action.ShouldThrowExactly<NoRegistrationForMessageException>().And.MessageType.Should().Be(typeof(TestCommand));
		}

		[Test]
		public async Task NoMatchingRegistrationWithANoMatchingRegistrationEventHandlerForAnObject() {

			var busSettings = new BusSettings();
			var globalPipelineProvider = Substitute.For<IGlobalPipelineProvider>();
			var globalPipelineTracker = Substitute.For<IGlobalPipelineTracker>();
			var dependencyScope = Substitute.For<IDependencyScope>();

			var handler = Substitute.For<IEventHandler<NoMatchingRegistrationEvent>>();

            globalPipelineProvider.GetGlobalPipeline().Returns(Pipeline.EmptyPipeline);
            dependencyScope.GetService(typeof(IEventHandler<NoMatchingRegistrationEvent>)).Returns(handler);

			var handlerProvider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(NoMatchingRegistrationEvent), typeof(IEventHandler<NoMatchingRegistrationEvent>))
			});

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			var runner = pipelineBuilder.GetPipelineForMessage(typeof(IMessage));

			await runner(Substitute.For<IMessage>());

			await handler.Received(1).Handle(Arg.Any<NoMatchingRegistrationEvent>());
		}

		[Test]
		public async Task NoMatchingRegistrationWithANoMatchingRegistrationEventHandlerForACommand()
		{
			var busSettings = new BusSettings();
			var globalPipelineProvider = Substitute.For<IGlobalPipelineProvider>();
			var globalPipelineTracker = Substitute.For<IGlobalPipelineTracker>();
			var dependencyScope = Substitute.For<IDependencyScope>();

			var handler = Substitute.For<IEventHandler<NoMatchingRegistrationEvent>>();

		    globalPipelineProvider.GetGlobalPipeline().Returns(Pipeline.EmptyPipeline);
            dependencyScope.GetService(typeof(IEventHandler<NoMatchingRegistrationEvent>)).Returns(handler);

			var handlerProvider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(NoMatchingRegistrationEvent), typeof(IEventHandler<NoMatchingRegistrationEvent>))
			});

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			var runner = pipelineBuilder.GetPipelineForMessage(typeof(TestCommand));

			await runner(new TestCommand());

			await handler.Received(1).Handle(Arg.Any<NoMatchingRegistrationEvent>());
		}
	}
}
