using System;

namespace Enexure.MicroBus.Sagas
{
	public class NoSagaFoundException : Exception
	{
		public NoSagaFoundException(Type sagaType, Type eventType)
			: base($"The event {eventType.FullName} cannot start the saga {sagaType.FullName}")
		{
		}
	}
}
