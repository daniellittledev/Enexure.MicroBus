using System;

namespace Enexure.MicroBus
{
	public class CommandBuilder<TCommand> : ICommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly BusBuilder busBuilder;

		public CommandBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TCommandHandler>() 
			where TCommandHandler : ICommandHandler<TCommand>
		{
			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), new Pipeline()));
		}

		public IBusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline));
		}
	}

	public class CommandBuilder : ICommandBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type commandType;

		public CommandBuilder(BusBuilder busBuilder, Type commandType)
		{
			this.busBuilder = busBuilder;
			this.commandType = commandType;
		}

		public IBusBuilder To(Type commandHandlerType)
		{
			return new BusBuilder(busBuilder, new MessageRegistration(commandType, commandHandlerType, new Pipeline()));
		}

		public IBusBuilder To(Type commandHandlerType, Pipeline pipeline)
		{
			return new BusBuilder(busBuilder, new MessageRegistration(commandType, commandHandlerType, pipeline));
		}
	}
}