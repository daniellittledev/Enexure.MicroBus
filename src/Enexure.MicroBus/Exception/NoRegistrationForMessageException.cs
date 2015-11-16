using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class NoRegistrationForMessageException : Exception
	{
		public NoRegistrationForMessageException(Type commandType)
			: base(string.Format("No registration for message of type {0} was found", commandType.Name))
		{
		}
	}
}