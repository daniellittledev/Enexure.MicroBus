using System.Threading.Tasks;
using Enexure.MicroBus.InfrastructureContracts;
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

		public async Task<object> Handle(IMessage message)
		{
			await innerHandler.Handle((TCommand)message);
			return null;
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

		public async Task<object> Handle(IMessage message)
		{
			await innerHandler.Handle((TEvent)message);
			return null;
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

		public async Task<object> Handle(IMessage message)
		{
			return await innerHandler.Handle((TQuery)message);
		}
	}
}