using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.NoDependencyInjection
{
	[TestFixture]
	public class EventTests
	{
		class Event : IEvent
		{
			public int Tally { get; set; }
		}

		class EventHandler : IEventHandler<Event>
		{
			public Task Handle(Event @event)
			{
				@event.Tally += 1;

				return Task.FromResult(0);
			}
		}

		class EventHandler2 : IEventHandler<Event>
		{
			public Task Handle(Event @event)
			{
				@event.Tally += 1;

				return Task.FromResult(0);
			}
		}

		[Test]
		public void NoHandlerShouldThrow()
		{
			var bus = new BusBuilder()
				.BuildBus();

			var func = (Func<Task>)(() => bus.Publish(new Event()));

			func.ShouldThrowExactly<NoRegistrationForMessageException>().WithMessage("No registration for message of type Event was found");
		}

		[Test]
		public async Task TestEvent()
		{
			var pipeline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var bus = new BusBuilder()
				.RegisterEvent<Event>().To(x => x.Handler<EventHandler>(), pipeline)
				.BuildBus();

			var @event = new Event();
			await bus.Publish(@event);
			
			@event.Tally.Should().Be(1);
		}

		[Test]
		public async Task TestMultipleEvents()
		{
			var pipeline = new Pipeline()
				.AddHandler<Common.PipelineHandler>();

			var bus = new BusBuilder()
				.RegisterEvent<Event>().To(x => {
					x.Handler<EventHandler>();
					x.Handler<EventHandler2>();
				}, pipeline)
				.BuildBus();

			var @event = new Event();
			await bus.Publish(@event);

			@event.Tally.Should().Be(2);
		}
	}
}
