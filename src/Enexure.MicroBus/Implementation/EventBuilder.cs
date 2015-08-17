using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class EventBuilder<TEvent> : IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly BusBuilder busBuilder;

		public EventBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TEventHandler>() where TEventHandler : IEventHandler<TEvent>
		{
			return To<TEventHandler>(new Pipeline());
		}

		public IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder)
		{
			return To(eventBinder, new Pipeline());
		}

		public IBusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			return To(new [] { typeof(TEventHandler) }, pipeline);
		}

		public IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			var binder = new EventBinder<TEvent>();
			eventBinder(binder);

			return To(binder.GetHandlerTypes(), pipeline);
		}

		private IBusBuilder To(IEnumerable<Type> handlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TEvent), handlerTypes, pipeline));
		}

	}

	public class EventBuilder : IEventBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type eventType;

		public EventBuilder(BusBuilder busBuilder, Type eventType)
		{
			this.busBuilder = busBuilder;
			this.eventType = eventType;
		}

		public IBusBuilder To(Type eventHandlerType)
		{
			return To(eventHandlerType, new Pipeline());
		}

		public IBusBuilder To(IEnumerable<Type> eventHandlerTypes)
		{
			return To(eventHandlerTypes, new Pipeline());
		}

		public IBusBuilder To(Type eventHandlerType, Pipeline pipeline)
		{
			return To(new [] { eventHandlerType }, new Pipeline());
		}

		public IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(eventType, eventHandlerTypes, pipeline));
		}
	}
}