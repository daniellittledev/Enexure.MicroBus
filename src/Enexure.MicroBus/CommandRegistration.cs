using System;

namespace Enexure.MicroBus
{
	public class CommandRegistration
	{
		private readonly Type commandType;
		private readonly Type commandHandlerType;
		private readonly Pipeline pipeline;

		public CommandRegistration(Type commandType, Type commandHandlerType, Pipeline pipeline)
		{
			this.commandType = commandType;
			this.commandHandlerType = commandHandlerType;
			this.pipeline = pipeline;
		}

		public Type CommandType
		{
			get { return commandType; }
		}

		public Type CommandHandlerType
		{
			get { return commandHandlerType; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}
	}
}