using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IEventBuilder<out TEvent>
		where TEvent : IEvent
	{
		IMessageRegister To<TEventHandler>()
			where TEventHandler : IEventHandler<TEvent>;

		IMessageRegister To(Action<IEventBinder<TEvent>> eventBinder);

		IMessageRegister To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>;

		IMessageRegister To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline);
	}

	public interface IEventBuilder
	{
		IMessageRegister To(Type eventHandlerType);
		IMessageRegister To(IEnumerable<Type> eventHandlerTypes);

		IMessageRegister To(Type eventHandlerType, Pipeline pipeline);
		IMessageRegister To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline);
	}
}