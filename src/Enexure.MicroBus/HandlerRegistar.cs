using System;
using System.Collections.Generic;
using System.Linq;
using Enexure.MicroBus;

namespace Enexure.MicroBus
{
	public class HandlerRegistar : IHandlerRegistar
	{
		private readonly IDictionary<Type, MessageRegistration> registrationsLookup;

		public HandlerRegistar(IEnumerable<MessageRegistration> registrations)
		{
			registrationsLookup = registrations.ToDictionary(x => x.MessageType, x => x);
		}

		public MessageRegistration GetRegistrationForMessage(Type commandType)
		{
			MessageRegistration value;
			if (registrationsLookup.TryGetValue(commandType, out value)) {
				return value;
			}

			throw new NoRegistrationForMessage(commandType);
		}

	}

	public class NoRegistrationForMessage : Exception
	{
		public NoRegistrationForMessage(Type commandType)
			: base(string.Format("No registration for message of type {0} was found", commandType.Name))
		{
		}
	}
}