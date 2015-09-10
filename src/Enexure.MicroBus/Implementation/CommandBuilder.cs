using System;

namespace Enexure.MicroBus
{
	public class CommandBuilder<TCommand> : ICommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly HandlerRegister messageRegister;

		public CommandBuilder(HandlerRegister messageRegister)
		{
			this.messageRegister = messageRegister;
		}

		public IMessageRegister To<TCommandHandler>() 
			where TCommandHandler : ICommandHandler<TCommand>
		{
			return new HandlerRegister(messageRegister, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), Pipeline.EmptyPipeline));
		}

		public IMessageRegister To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(messageRegister, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline));
		}
	}

	public class CommandBuilder : ICommandBuilder
	{
		private readonly HandlerRegister messageRegister;
		private readonly Type commandType;

		public CommandBuilder(HandlerRegister messageRegister, Type commandType)
		{
			this.messageRegister = messageRegister;
			this.commandType = commandType;
		}

		public IMessageRegister To(Type commandHandlerType)
		{
			return new HandlerRegister(messageRegister, new MessageRegistration(commandType, commandHandlerType, Pipeline.EmptyPipeline));
		}

		public IMessageRegister To(Type commandHandlerType, Pipeline pipeline)
		{
			return new HandlerRegister(messageRegister, new MessageRegistration(commandType, commandHandlerType, pipeline));
		}
	}
}