using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IHandlerBuilder
	{
		ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand;

		IEventHandler<TEvent> GetRunnerForEvent<TEvent>()
			where TEvent : IEvent;

		IQueryHandler<TQuery, TResult> GetRunnerForQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult;
	}
}