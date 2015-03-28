using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IHandlerBuilder
	{
		ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand;
	}
}