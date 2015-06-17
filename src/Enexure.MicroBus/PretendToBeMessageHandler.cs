using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	internal class PretendToBeCommandHandler<TCommand> : ICommandHandler<TCommand>
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

	internal class PretendToBeEventHandler<TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly IPipelineHandler innerHandler;

		public PretendToBeEventHandler(IPipelineHandler innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(TEvent @event)
		{
			return innerHandler.Handle(@event);
		}
	}

	internal class PretendToBeQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly IPipelineHandler innerHandler;

		public PretendToBeQueryHandler(IPipelineHandler innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public async Task<TResult> Handle(TQuery query)
		{
			return (TResult) await innerHandler.Handle(query);
		}
	}
}