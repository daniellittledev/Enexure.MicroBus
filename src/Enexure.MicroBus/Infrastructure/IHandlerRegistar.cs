using System;

namespace Enexure.MicroBus
{
	public interface IHandlerRegistar
	{
		CommandRegistration GetRegistrationFor(Type commandType);
	}
}