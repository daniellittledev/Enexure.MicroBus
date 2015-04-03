using System;

namespace Enexure.MicroBus
{
	public interface IHandlerRegistar
	{
		MessageRegistration GetRegistrationForMessage(Type commandType);
	}
}