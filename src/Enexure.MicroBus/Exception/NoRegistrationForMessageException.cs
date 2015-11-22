using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class NoRegistrationForMessageException : Exception
	{
		private readonly Type messageType;

		public NoRegistrationForMessageException(Type messageType)
			: base(string.Format("No registration for message of type {0} was found", messageType.Name))
		{
			this.messageType = messageType;
		}

		public Type MessageType {
			get {
				return messageType;
			}
		}
	}
}