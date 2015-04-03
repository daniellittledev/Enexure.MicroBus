using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests
{
	class Event : IEvent {}

	class EventHandler : IEventHandler<Event>
	{
		public Task Handle(Event command)
		{
			return Task.FromResult(0);
		}
	}

	[TestFixture]
	public class EventTests
	{
		[Test]
		public async Task TestEvent()
		{
			var pipline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var bus = new BusBuilder()
				.Register<EventHandler>(pipline)
				.BuildBus();

			await bus.Publish(new Event());

		}
	}
}
