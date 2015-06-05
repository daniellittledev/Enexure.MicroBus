using System;
using Enexure.MicroBus.InfrastructureContracts;

namespace Enexure.MicroBus.Autofac
{
	public class MessageRegistration
	{
		private readonly Type messageType;
		private readonly Type messageHandlerType;
		private readonly Pipeline pipeline;

		public MessageRegistration(Type messageType, Type messageHandlerType, Pipeline pipeline)
		{
			this.messageType = messageType;
			this.messageHandlerType = messageHandlerType;
			this.pipeline = pipeline;
		}

		public Type MessageType
		{
			get { return messageType; }
		}

		public Type MessageHandlerType
		{
			get { return messageHandlerType; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}
	}
}