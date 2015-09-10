using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class EventBuilder<TEvent> : IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly HandlerRegister messageRegister;

		public EventBuilder(HandlerRegister messageRegister)
		{
			this.messageRegister = messageRegister;
		}

		public IMessageRegister To<TEventHandler>() where TEventHandler : IEventHandler<TEvent>
		{
			return To<TEventHandler>(Pipeline.EmptyPipeline);
		}

		public IMessageRegister To(Action<IEventBinder<TEvent>> eventBinder)
		{
			return To(eventBinder, Pipeline.EmptyPipeline);
		}

		public IMessageRegister To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			return To(new [] { typeof(TEventHandler) }, pipeline);
		}

		public IMessageRegister To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			var binder = new EventBinder<TEvent>();
			eventBinder(binder);

			return To(binder.GetHandlerTypes(), pipeline);
		}

		private IMessageRegister To(IEnumerable<Type> handlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(messageRegister, handlerTypes.Select(x => new MessageRegistration(typeof(TEvent), x, pipeline)));
		}

	}

	public class EventBuilder : IEventBuilder
	{
		private readonly HandlerRegister messageRegister;
		private readonly Type eventType;

		public EventBuilder(HandlerRegister messageRegister, Type eventType)
		{
			this.messageRegister = messageRegister;
			this.eventType = eventType;
		}

		public IMessageRegister To(Type eventHandlerType)
		{
			return To(eventHandlerType, Pipeline.EmptyPipeline);
		}

		public IMessageRegister To(IEnumerable<Type> eventHandlerTypes)
		{
			return To(eventHandlerTypes, Pipeline.EmptyPipeline);
		}

		public IMessageRegister To(Type eventHandlerType, Pipeline pipeline)
		{
			return To(new [] { eventHandlerType }, Pipeline.EmptyPipeline);
		}

		public IMessageRegister To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(messageRegister, eventHandlerTypes.Select(x => new MessageRegistration(eventType, x, pipeline)));
		}
	}
}