using System;
using System.Threading.Tasks;
using Enexure.MicroBus;

namespace Enexure.MicroBus
{
	public class MicroBus : IMicroBus
	{
		private readonly IHandlerBuilder registrations;

		public MicroBus(IHandlerBuilder registrations)
		{
			this.registrations = registrations;
		}

		public Task Send<TCommand>(TCommand busCommand)
			where TCommand : ICommand
		{
			var handler = registrations.GetRunnerForCommand<TCommand>();
			return handler.Handle(busCommand);
		}

		public Task Publish<TEvent>(TEvent @event)
			where TEvent : IEvent 
		{
			var handler = registrations.GetRunnerForEvent<TEvent>();
			return handler.Handle(@event);
		}

		public Task<TResult> Query<TQuery, TResult>(IQuery<TQuery, TResult> query)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			var handler = registrations.GetRunnerForQuery<TQuery, TResult>();
			return handler.Handle((TQuery)query);
		}
	}
}
