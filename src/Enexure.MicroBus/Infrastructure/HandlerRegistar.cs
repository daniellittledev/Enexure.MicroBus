using System;
using System.Collections.Generic;
using System.Linq;

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
			return registrationsLookup[commandType];
		}

	}
}