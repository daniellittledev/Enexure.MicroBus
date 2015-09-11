using System;

namespace Enexure.MicroBus
{
	public interface ICommandBuilder<out TCommand>
		where TCommand : ICommand
	{
		IHandlerRegister To<TCommandHandler>()
			where TCommandHandler : ICommandHandler<TCommand>;

		IHandlerRegister To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>;
	}

	public interface ICommandBuilder
	{
		IHandlerRegister To(Type commandHandlerType);
		IHandlerRegister To(Type commandHandlerType, Pipeline pipeline);
	}
}