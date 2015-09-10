using System;

namespace Enexure.MicroBus
{
	public class MessageRegistration
	{
		private readonly Type messageType;
		private readonly Pipeline pipeline;
		private readonly Type handler;

		public MessageRegistration(Type messageType, Type handlerType, Pipeline pipeline)
		{
			if (messageType == null) throw new ArgumentNullException(nameof(messageType));
			if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
			if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

			// TODO: messageType should implement IMessage
			// TODO: handlerType should be a handler

			this.messageType = messageType;
			this.pipeline = pipeline;
			this.handler = handlerType;
		}

		public Type MessageType
		{
			get { return messageType; }
		}

		public Type Handler
		{
			get { return handler; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}

	}
}