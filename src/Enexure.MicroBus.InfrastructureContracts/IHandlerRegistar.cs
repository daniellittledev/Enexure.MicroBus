using System;

namespace Enexure.MicroBus.InfrastructureContracts
{
	public interface IHandlerRegistar
	{
		MessageRegistration GetRegistrationForMessage(Type commandType);
	}
}