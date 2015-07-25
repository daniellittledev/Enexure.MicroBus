using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public class MicroBus : IMicroBus
	{
		private readonly IHandlerBuilder registrations;
		private readonly IDependencyResolver dependencyResolver;

		public MicroBus(IHandlerBuilder registrations, IDependencyResolver dependencyResolver)
		{
			this.registrations = registrations;
			this.dependencyResolver = dependencyResolver;
		}

		public Task Send<TCommand>(TCommand busCommand)
			where TCommand : ICommand
		{
			using (var scope = dependencyResolver.BeginScope()) {
				return registrations.GetRunnerForCommand<TCommand>(scope)(busCommand);
			}
		}

		public Task Publish<TEvent>(TEvent busEvent)
			where TEvent : IEvent 
		{
			using (var scope = dependencyResolver.BeginScope()) {
				return registrations.GetRunnerForEvent<TEvent>(scope)(busEvent);
			}
		}

		public Task<TResult> Query<TQuery, TResult>(IQuery<TQuery, TResult> busQuery)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			using (var scope = dependencyResolver.BeginScope()) {
				return registrations.GetRunnerForQuery<TQuery, TResult>(scope)((TQuery)busQuery);
			}
		}
	}
}
