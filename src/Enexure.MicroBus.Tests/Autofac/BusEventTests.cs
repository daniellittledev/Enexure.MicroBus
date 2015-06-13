using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;
using Enexure.MicroBus.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.Autofac
{
	[TestFixture]
	public class AutofacEventTests
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
		public async Task TestEvent()
		{
			var pipline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var bus = new BusBuilder()
				.RegisterEvent<Event>().To(x => x.Handler<EventHandler>(), pipline)
				.BuildBus();

			var @event = new Event();
			await bus.Publish(@event);
			
			@event.Tally.Should().Be(1);
		}

		[Test]
		public async Task TestMultipleEvents()
		{
			var pipline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var bus = new BusBuilder()
				.RegisterEvent<Event>().To(x => {
					x.Handler<EventHandler>();
					x.Handler<EventHandler2>();
				}, pipline)
				.BuildBus();

			var @event = new Event();
			await bus.Publish(@event);

			@event.Tally.Should().Be(2);
		}
	}
}
