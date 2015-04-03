using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	internal class CommandHandlerPretendToBePipelineHandler<TCommand> : IPipelineHandler
		where TCommand : ICommand
	{
		private readonly ICommandHandler<TCommand> innerHandler;

		public CommandHandlerPretendToBePipelineHandler(ICommandHandler<TCommand> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(IMessage message)
		{
			return innerHandler.Handle((TCommand)message);
		}
	}

	internal class EventHandlerPretendToBePipelineHandler<TEvent> : IPipelineHandler
		where TEvent : IEvent
	{
		private readonly IEventHandler<TEvent> innerHandler;

		public EventHandlerPretendToBePipelineHandler(IEventHandler<TEvent> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(IMessage message)
		{
			return innerHandler.Handle((TEvent)message);
		}
	}

	internal class QueryHandlerPretendToBePipelineHandler<TQuery, TResult> : IPipelineHandler
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly IQueryHandler<TQuery, TResult> innerHandler;

		public QueryHandlerPretendToBePipelineHandler(IQueryHandler<TQuery, TResult> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(IMessage message)
		{
			return innerHandler.Handle((TQuery)message);
		}
	}
}