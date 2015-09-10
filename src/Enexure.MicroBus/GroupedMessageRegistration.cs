using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class GroupedMessageRegistration
	{
		private readonly Pipeline pipeline;
		private readonly IReadOnlyCollection<Type> handler;

		public GroupedMessageRegistration(Pipeline pipeline, IReadOnlyCollection<Type> handlerType)
		{
			this.pipeline = pipeline;
			this.handler = handlerType;
		}

		public IReadOnlyCollection<Type> Handlers
		{
			get { return handler; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}

	}
}