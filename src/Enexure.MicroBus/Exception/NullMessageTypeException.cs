using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class NullMessageTypeException : Exception
	{
		public NullMessageTypeException(Type type)
			: base(string.Format("Message was null but an instance of type '{0}' was expected", type.Name))
		{
		}

		public NullMessageTypeException()
			: base("Message was null")
		{
		}
	}
}