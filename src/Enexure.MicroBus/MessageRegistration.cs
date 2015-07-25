using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class MessageRegistration
	{
		private readonly MessageType type;
		private readonly Type messageType;
		private readonly Pipeline pipeline;
		private readonly IEnumerable<Type> handlers;

		public MessageRegistration(Type messageType, Type messageHandlerType, Pipeline pipeline)
		{
			this.type = GetMessageType(messageType);
			this.messageType = messageType;
			this.pipeline = pipeline;
			this.handlers = new[] { messageHandlerType };
		}

		public MessageRegistration(Type messageType, IEnumerable<Type> handlers, Pipeline pipeline)
		{
			this.type = GetMessageType(messageType);
			this.messageType = messageType;
			this.pipeline = pipeline;
			this.handlers = handlers;
		}

		private static MessageType GetMessageType(Type messageType)
		{
			if (typeof(ICommand).IsAssignableFrom(messageType)) {
				return Enexure.MicroBus.MessageType.Command;

			} else if (typeof(IEvent).IsAssignableFrom(messageType)) {
				return Enexure.MicroBus.MessageType.Event;

			} else if (messageType.GetInterfaces()
				.Where(i => i.IsGenericType)
				.Any(i => i.GetGenericTypeDefinition() == typeof(IQuery<,>))) {
					return Enexure.MicroBus.MessageType.Query;

			} else {
				throw new NotSupportedException(string.Format("The message type {0} is not supported", messageType));
			}
		}

		public Type MessageType
		{
			get { return messageType; }
		}

		public IEnumerable<Type> Handlers
		{
			get { return handlers; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}

		public MessageType Type
		{
			get { return type; }
		}
	}
}