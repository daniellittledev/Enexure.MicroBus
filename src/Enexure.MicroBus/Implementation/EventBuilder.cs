using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class EventBuilder<TEvent> : IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly HandlerRegister handlerRegister;

		public EventBuilder(HandlerRegister handlerRegister)
		{
			this.handlerRegister = handlerRegister;
		}

		public IHandlerRegister To<TEventHandler>() where TEventHandler : IEventHandler<TEvent>
		{
			return To<TEventHandler>(Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To(Action<IEventBinder<TEvent>> eventBinder)
		{
			return To(eventBinder, Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			return To(new [] { typeof(TEventHandler) }, pipeline);
		}

		public IHandlerRegister To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			var binder = new EventBinder<TEvent>();
			eventBinder(binder);

			return To(binder.GetHandlerTypes(), pipeline);
		}

		private IHandlerRegister To(IEnumerable<Type> handlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(handlerRegister, handlerTypes.Select(x => new MessageRegistration(typeof(TEvent), x, pipeline)));
		}

	}

	public class EventBuilder : IEventBuilder
	{
		private readonly HandlerRegister handlerRegister;
		private readonly Type eventType;

		public EventBuilder(HandlerRegister handlerRegister, Type eventType)
		{
			this.handlerRegister = handlerRegister;
			this.eventType = eventType;
		}

		public IHandlerRegister To(Type eventHandlerType)
		{
			return To(eventHandlerType, Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To(IEnumerable<Type> eventHandlerTypes)
		{
			return To(eventHandlerTypes, Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To(Type eventHandlerType, Pipeline pipeline)
		{
			return To(new [] { eventHandlerType }, Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(handlerRegister, eventHandlerTypes.Select(x => new MessageRegistration(eventType, x, pipeline)));
		}
	}
}