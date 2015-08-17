using System;

namespace Enexure.MicroBus
{
	public interface ICommandBuilder<out TCommand>
		where TCommand : ICommand
	{
		IBusBuilder To<TCommandHandler>()
			where TCommandHandler : ICommandHandler<TCommand>;

		IBusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>;
	}

	public interface ICommandBuilder
	{
		IBusBuilder To(Type commandHandlerType);
		IBusBuilder To(Type commandHandlerType, Pipeline pipeline);
	}
}