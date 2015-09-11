using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class HandlerRegister : IHandlerRegister
	{
		private readonly ImmutableList<MessageRegistration> registrations;

		public HandlerRegister()
		{
			registrations = ImmutableList<MessageRegistration>.Empty;
		}

		public HandlerRegister(IEnumerable<MessageRegistration> registrations)
		{
			this.registrations = registrations as ImmutableList<MessageRegistration> ?? ImmutableList<MessageRegistration>.Empty.AddRange(registrations);
		}

		public HandlerRegister(HandlerRegister handlerRegister, MessageRegistration registration)
		{
			this.registrations = handlerRegister.registrations.Add(registration);
		}

		public HandlerRegister(HandlerRegister handlerRegister, IEnumerable<MessageRegistration> registrations)
		{
			this.registrations = handlerRegister.registrations.AddRange(registrations);
		}

		public ICommandBuilder<TCommand> RegisterCommand<TCommand>()
			where TCommand : ICommand
		{
			return new CommandBuilder<TCommand>(this);
		}

		public ICommandBuilder RegisterCommand(Type type)
		{
			return new CommandBuilder(this, type);
		}

		public IEventBuilder<TEvent> RegisterEvent<TEvent>()
			where TEvent : IEvent
		{
			return new EventBuilder<TEvent>(this);
		}

		public IEventBuilder RegisterEvent(Type type)
		{
			return new EventBuilder(this, type);
		}

		public IQueryBuilder<TQuery, TResult> RegisterQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return new QueryBuilder<TQuery, TResult>(this);
		}

		public IQueryBuilder RegisterQuery(Type type)
		{
			return new QueryBuilder(this, type);
		}

		public IReadOnlyCollection<MessageRegistration> GetMessageRegistrations()
		{
			return registrations;
		}

	}
}
