using Enexure.MicroBus.Messages;
using System.Reflection;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace Enexure.MicroBus.Tests.PipelineBuilderTests
{
	class TestGlobalPipelineProvider : IGlobalPipelineProvider
	{
		public Pipeline GetGlobalPipeline()
		{
			return Pipeline.EmptyPipeline;
		}
	}

	class TestGlobalPipelineTracker : IGlobalPipelineTracker
	{
		bool hasRun = false;

		public bool HasRun
		{
			get
			{
				return hasRun;
			}
		}

		public void MarkAsRun()
		{
			hasRun = true;
		}
	}

	class TestDependencyScope : IDependencyScope
	{
		object value;

		public TestDependencyScope(object value)
		{
			this.value = value;
		}

		public IDependencyScope BeginScope()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public object GetService(Type serviceType)
		{
			if (serviceType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
			{
				return value;
			}

			throw new NotImplementedException();
		}

		public T GetService<T>()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetServices<T>()
		{
			throw new NotImplementedException();
		}
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

	public class NoMessageRegistrationTests
	{
		class TestCommand : ICommand
		{ }

		[Fact]
		public void NoMatchingRegistrationWithNoNoMatchingRegistrationEventHandler()
		{
			var busSettings = new BusSettings();
			var globalPipelineProvider = new TestGlobalPipelineProvider();
			var globalPipelineTracker = new TestGlobalPipelineTracker();
			var dependencyScope = new TestDependencyScope(null);

			var handlerProvider = HandlerProvider.Create(new MessageRegistration[] {});

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			Action action = () => pipelineBuilder.GetPipelineForMessage(typeof(TestCommand));

			action.ShouldThrowExactly<NoRegistrationForMessageException>().And.MessageType.Should().Be(typeof(TestCommand));
		}

		[Fact]
		public async Task NoMatchingRegistrationWithANoMatchingRegistrationEventHandlerForAnObject() {

			var busSettings = new BusSettings();
			var globalPipelineProvider = new TestGlobalPipelineProvider();
			var globalPipelineTracker = new TestGlobalPipelineTracker();
			var dependencyScope = new TestDependencyScope(new TestEventHandler());

			var handlerProvider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(NoMatchingRegistrationEvent), typeof(IEventHandler<NoMatchingRegistrationEvent>))
			});

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			var runner = pipelineBuilder.GetPipelineForMessage(typeof(IMessage));

			await runner(new CommandA());

			var handler = (TestEventHandler)dependencyScope.GetService(typeof(IEventHandler<NoMatchingRegistrationEvent>));
			handler.CallsToHandle.Should().Be(1);
		}

		[Fact]
		public async Task NoMatchingRegistrationWithANoMatchingRegistrationEventHandlerForACommand()
		{
			var busSettings = new BusSettings();
			var globalPipelineProvider = new TestGlobalPipelineProvider();
			var globalPipelineTracker = new TestGlobalPipelineTracker();
			var dependencyScope = new TestDependencyScope(new TestEventHandler());

			var handlerProvider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(NoMatchingRegistrationEvent), typeof(IEventHandler<NoMatchingRegistrationEvent>))
			});

			var pipelineBuilder = new PipelineBuilder(busSettings, handlerProvider, globalPipelineProvider, globalPipelineTracker, dependencyScope);

			var runner = pipelineBuilder.GetPipelineForMessage(typeof(TestCommand));

			await runner(new TestCommand());

			var handler = (TestEventHandler)dependencyScope.GetService(typeof(IEventHandler<NoMatchingRegistrationEvent>));
			handler.CallsToHandle.Should().Be(1);
		}
	}
}
