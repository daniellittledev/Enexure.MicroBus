using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class HandlerRegistar : IHandlerRegistar
	{
		private readonly IDictionary<Type, CommandRegistration> commandRegistrationsLookup;

		public HandlerRegistar(IEnumerable<CommandRegistration> commandRegistrations)
		{
			commandRegistrationsLookup = commandRegistrations.ToDictionary(x => x.CommandType, x => x);
		}

		public CommandRegistration GetRegistrationFor(Type commandType)
		{
			return commandRegistrationsLookup[commandType];
		}
	}
}