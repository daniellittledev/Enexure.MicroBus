using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	internal class PretendMultipleToBeEventHandler<TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly IEnumerable<IEventHandler<TEvent>> eventHandlers;

		public PretendMultipleToBeEventHandler(IEnumerable<IEventHandler<TEvent>> eventHandlers)
		{
			this.eventHandlers = eventHandlers;
		}

		public Task Handle(TEvent @event)
		{
			return Task.WhenAll(eventHandlers.Select(x => x.Handle(@event)));
		}
	}
}