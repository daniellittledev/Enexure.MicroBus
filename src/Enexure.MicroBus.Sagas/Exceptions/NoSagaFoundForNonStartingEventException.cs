using System;

namespace Enexure.MicroBus.Sagas
{
	public class NoSagaFoundForNonStartingEventException : Exception
	{
		public NoSagaFoundForNonStartingEventException(Type sagaType, Type eventType)
			: base($"The event {eventType.FullName} cannot start the saga {sagaType.FullName}")
		{
		}
	}
}
