using System;

namespace Enexure.MicroBus
{
	public class MultipleRegistrationsWithTheSameMessageException : Exception
	{
		public MultipleRegistrationsWithTheSameMessageException(string messageTypeName, Type messageType)
			: base($"More than one handler was registered to the {messageTypeName} {messageType.FullName}. Cannot register two handlers to a single {messageTypeName}.")
		{
		}
	}
}