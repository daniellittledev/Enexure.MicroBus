using System;

namespace Enexure.MicroBus
{
	public class MultipleRegistrationsWithTheSameCommandException : MultipleRegistrationsWithTheSameMessageException
	{
		public MultipleRegistrationsWithTheSameCommandException(Type messageType)
			: base("command", messageType)
		{
		}
	}
}