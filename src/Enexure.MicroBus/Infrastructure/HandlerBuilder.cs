using System.Linq;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class HandlerBuilder : IHandlerBuilder
	{
		private readonly IHandlerActivator handlerActivator;
		private readonly IHandlerRegistar handlerRegistar;

		public HandlerBuilder(IHandlerActivator handlerActivator, IHandlerRegistar handlerRegistar)
		{
			this.handlerActivator = handlerActivator;
			this.handlerRegistar = handlerRegistar;
		}

		public ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand
		{
			var registration = handlerRegistar.GetRegistrationFor(typeof(TCommand));

			var pipeline = registration.Pipeline;
			var leafHandler = registration.CommandHandlerType;

			var innerCommandHandler = handlerActivator.ActivateHandler<ICommandHandler<TCommand>>(leafHandler);

			var handler = pipeline.Aggregate(
				(IPipelineHandler)new PretendToBePipelineHandler<TCommand>(innerCommandHandler),
				(current, handlerType) => handlerActivator.ActivateHandler<IPipelineHandler>(handlerType, current));

			return new PretendToBeCommandHandler<TCommand>(handler);
		}
	}
}