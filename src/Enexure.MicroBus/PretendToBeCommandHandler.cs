using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class PretendToBeCommandHandler<TCommand> : ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		private readonly IPipelineHandler innerHandler;

		public PretendToBeCommandHandler(IPipelineHandler innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(TCommand command)
		{
			return innerHandler.Handle(command);
		}
	}
}