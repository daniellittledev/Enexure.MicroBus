using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class MicroBus : IBus
	{
		private readonly IBusRegistrations registrations;

		public MicroBus(IBusRegistrations registrations)
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
			throw new NotImplementedException();
		}

		public Task<TResult> Query<TQuery, TResult>(TQuery query)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			throw new NotImplementedException();
		}
	}
}
