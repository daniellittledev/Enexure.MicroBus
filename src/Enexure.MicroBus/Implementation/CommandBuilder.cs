using System;

namespace Enexure.MicroBus
{
	public class CommandBuilder<TCommand> : ICommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly HandlerRegister handlerRegister;

		public CommandBuilder(HandlerRegister handlerRegister)
		{
			this.handlerRegister = handlerRegister;
		}

		public IHandlerRegister To<TCommandHandler>() 
			where TCommandHandler : ICommandHandler<TCommand>
		{
			return new HandlerRegister(handlerRegister, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), Pipeline.EmptyPipeline));
		}

		public IHandlerRegister To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(handlerRegister, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline));
		}
	}

	public class CommandBuilder : ICommandBuilder
	{
		private readonly HandlerRegister handlerRegister;
		private readonly Type commandType;

		public CommandBuilder(HandlerRegister handlerRegister, Type commandType)
		{
			this.handlerRegister = handlerRegister;
			this.commandType = commandType;
		}

		public IHandlerRegister To(Type commandHandlerType)
		{
			return new HandlerRegister(handlerRegister, new MessageRegistration(commandType, commandHandlerType, Pipeline.EmptyPipeline));
		}

		public IHandlerRegister To(Type commandHandlerType, Pipeline pipeline)
		{
			return new HandlerRegister(handlerRegister, new MessageRegistration(commandType, commandHandlerType, pipeline));
		}
	}
}