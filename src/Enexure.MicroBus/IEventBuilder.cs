using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IEventBuilder<out TEvent>
		where TEvent : IEvent
	{
		IHandlerRegister To<TEventHandler>()
			where TEventHandler : IEventHandler<TEvent>;

		IHandlerRegister To(Action<IEventBinder<TEvent>> eventBinder);

		IHandlerRegister To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>;

		IHandlerRegister To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline);
	}

	public interface IEventBuilder
	{
		IHandlerRegister To(Type eventHandlerType);
		IHandlerRegister To(IEnumerable<Type> eventHandlerTypes);

		IHandlerRegister To(Type eventHandlerType, Pipeline pipeline);
		IHandlerRegister To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline);
	}
}