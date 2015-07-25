using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IHandlerBuilder
	{
		Func<TCommand, Task> GetRunnerForCommand<TCommand>(IDependencyScope scope)
			where TCommand : ICommand;

		Func<TEvent, Task> GetRunnerForEvent<TEvent>(IDependencyScope scope)
			where TEvent : IEvent;

		Func<TQuery, Task<TResult>> GetRunnerForQuery<TQuery, TResult>(IDependencyScope scope)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult;
	}
}