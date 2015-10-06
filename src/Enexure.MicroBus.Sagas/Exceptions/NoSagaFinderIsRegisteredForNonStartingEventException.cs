using System;

namespace Enexure.MicroBus.Sagas
{
	public class NoSagaFinderRegisteredException : Exception
	{
		public NoSagaFinderRegisteredException(Type sagaType, Type eventType)
			: base($"No saga finder is registered for the event {eventType.FullName} and the start the saga {sagaType.FullName}")
		{
		}
	}
}
