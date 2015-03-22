using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IBus
	{
		Task Send<TCommand>(TCommand busCommand)
			where TCommand : ICommand;

		Task Publish<TEvent>(TEvent @event)
			where TEvent : IEvent;

		Task<TResult> Query<TQuery, TResult>(TQuery query)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult;
	}
}
