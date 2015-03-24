using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class PretendToBePipelineHandler<TCommand> : IPipelineHandler
		where TCommand : ICommand
	{
		private readonly ICommandHandler<TCommand> innerHandler;

		public PretendToBePipelineHandler(ICommandHandler<TCommand> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(IMessage message)
		{
			return innerHandler.Handle((TCommand)message);
		}
	}
}