using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	internal class EventPretendToBeCommandHandler<TCommand> : ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		private readonly IEventHandler<NoMatchingRegistrationEvent> innerHandler;

		public EventPretendToBeCommandHandler(IEventHandler<NoMatchingRegistrationEvent> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(TCommand command)
		{
			return innerHandler.Handle(new NoMatchingRegistrationEvent(command));
		}
	}

	internal class EventPretendToBeEventHandler<TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly IEventHandler<NoMatchingRegistrationEvent> innerHandler;

		public EventPretendToBeEventHandler(IEventHandler<NoMatchingRegistrationEvent> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task Handle(TEvent @event)
		{
			return innerHandler.Handle(new NoMatchingRegistrationEvent(@event));
		}
	}

	internal class EventPretendToBeQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly IEventHandler<NoMatchingRegistrationEvent> innerHandler;

		public EventPretendToBeQueryHandler(IEventHandler<NoMatchingRegistrationEvent> innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public async Task<TResult> Handle(TQuery query)
		{
			await innerHandler.Handle(new NoMatchingRegistrationEvent(query));
			return default(TResult);
		}
	}
}