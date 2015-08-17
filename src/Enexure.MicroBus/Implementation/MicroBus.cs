using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public class MicroBus : IMicroBus
	{
		private readonly IPipelineBuilder registrations;
		private readonly IDependencyResolver dependencyResolver;

		public MicroBus(IPipelineBuilder registrations, IDependencyResolver dependencyResolver)
		{
			this.registrations = registrations;
			this.dependencyResolver = dependencyResolver;
		}

		public Task Send(ICommand busCommand)
		{
			if (busCommand == null) throw new ArgumentNullException("busCommand");

			using (var scope = dependencyResolver.BeginScope()) {
				return registrations.GetPipelineForMessage(scope, busCommand.GetType())(busCommand);
			}
		}

		public Task Publish(IEvent busEvent)
		{
			if (busEvent == null) throw new ArgumentNullException("busEvent");

			using (var scope = dependencyResolver.BeginScope()) {
				return registrations.GetPipelineForMessage(scope, busEvent.GetType())(busEvent);
			}
		}

		public async Task<TResult> Query<TQuery, TResult>(IQuery<TQuery, TResult> busQuery)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			if (busQuery == null) throw new ArgumentNullException("busQuery");

			using (var scope = dependencyResolver.BeginScope()) {
				var result = await registrations.GetPipelineForMessage(scope, busQuery.GetType())((TQuery)busQuery);
			    return (TResult) result;
			}
		}
	}
}
