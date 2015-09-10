using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public interface IHandlerProvider
	{
		bool GetRegistrationForMessage(Type commandType, out GroupedMessageRegistration registration);
	}
}