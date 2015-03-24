using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IBusRegistrations
	{
		ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand;
	}
}