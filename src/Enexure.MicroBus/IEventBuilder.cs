using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IEventBuilder<out TEvent>
		where TEvent : IEvent
	{
		IBusBuilder To<TEventHandler>()
			where TEventHandler : IEventHandler<TEvent>;

		IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder);

		IBusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>;

		IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline);
	}

	public interface IEventBuilder
	{
		IBusBuilder To(Type eventHandlerType);
		IBusBuilder To(IEnumerable<Type> eventHandlerTypes);

		IBusBuilder To(Type eventHandlerType, Pipeline pipeline);
		IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline);
	}
}