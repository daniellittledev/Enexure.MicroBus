using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class EventBinder<TEvent> : IEventBinder<TEvent>
		where TEvent : IEvent
	{
		private readonly List<Type> eventTypes = new List<Type>();

		public IEventBinder<TEvent> Handler<THandler>()
			where THandler : IEventHandler<TEvent>
		{
			eventTypes.Add(typeof(THandler));

			return this;
		}

		internal IEnumerable<Type> GetHandlerTypes()
		{
			return eventTypes;
		}
	}
}