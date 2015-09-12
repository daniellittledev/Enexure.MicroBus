using System;

namespace Enexure.MicroBus
{
	public class MultipleRegistrationsWithTheSameQueryException : MultipleRegistrationsWithTheSameMessageException
	{
		public MultipleRegistrationsWithTheSameQueryException(Type messageType)
			: base("query", messageType)
		{
		}
	}
}