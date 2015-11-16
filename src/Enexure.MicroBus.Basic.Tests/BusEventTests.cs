using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests
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
			var bus = BusBuilder.BuildBus(b => b);

			var func = (Func<Task>)(() => bus.Publish(new Event()));

			func.ShouldThrowExactly<NoRegistrationForMessageException>().WithMessage("No registration for message of type Event was found");
		}

		[Test]
		public async Task TestEvent()
		{
			var bus = BusBuilder.BuildBus(b =>
				b.RegisterEvent<Event>().To(x => x.Handler<EventHandler>())
			);

			var @event = new Event();
			await bus.Publish(@event);
			
			@event.Tally.Should().Be(1);
		}

		[Test]
		public async Task TestMultipleEvents()
		{
		    var bus = BusBuilder.BuildBus(b =>
		        b.RegisterEvent<Event>().To(x => {
		            x.Handler<EventHandler>();
		            x.Handler<EventHandler2>();
		        })
		    );

			var @event = new Event();
			await bus.Publish(@event);

			@event.Tally.Should().Be(2);
		}
	}
}
