using System;

namespace Enexure.MicroBus
{
	public interface ICommandBuilder<out TCommand>
		where TCommand : ICommand
	{
		IMessageRegister To<TCommandHandler>()
			where TCommandHandler : ICommandHandler<TCommand>;

		IMessageRegister To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>;
	}

	public interface ICommandBuilder
	{
		IMessageRegister To(Type commandHandlerType);
		IMessageRegister To(Type commandHandlerType, Pipeline pipeline);
	}
}