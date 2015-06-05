using System;
using System.Collections.Generic;

namespace Enexure.MicroBus.InfrastructureContracts
{
	public class MessageRegistration
	{
		private readonly Type messageType;
		private readonly Pipeline pipeline;
		private readonly IEnumerable<Type> handlers;

		public MessageRegistration(Type messageType, Type messageHandlerType, Pipeline pipeline)
		{
			this.messageType = messageType;
			this.pipeline = pipeline;
			this.handlers = new[] { messageHandlerType };
		}

		public MessageRegistration(Type messageType, IEnumerable<Type> handlers, Pipeline pipeline)
		{
			this.messageType = messageType;
			this.pipeline = pipeline;
			this.handlers = handlers;
		}

		public Type MessageType
		{
			get { return messageType; }
		}

		public IEnumerable<Type> Handlerss
		{
			get { return handlers; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}
	}
}