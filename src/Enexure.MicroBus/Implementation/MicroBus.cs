using Enexure.MicroBus.Annotations;
using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public class MicroBus : IMicroBus
	{
		private readonly IDependencyResolver dependencyResolver;

		public MicroBus([NotNull]IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));

			this.dependencyResolver = dependencyResolver;
		}

		public async Task Send(ICommand busCommand)
		{
			if (busCommand == null) throw new ArgumentNullException(nameof(busCommand));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineBuilder>();
				var messageProcessor = builder.GetPipelineForMessage(busCommand.GetType());
				await messageProcessor(busCommand);
			}
		}

		public async Task Publish(IEvent busEvent)
		{
			if (busEvent == null) throw new ArgumentNullException(nameof(busEvent));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineBuilder>();
				var messageProcessor = builder.GetPipelineForMessage(busEvent.GetType());
				await messageProcessor(busEvent);
			}
		}

		public async Task<TResult> Query<TQuery, TResult>(IQuery<TQuery, TResult> busQuery)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			if (busQuery == null) throw new ArgumentNullException(nameof(busQuery));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineBuilder>();
				var messageProcessor = builder.GetPipelineForMessage(busQuery.GetType());
				return (TResult) await messageProcessor(busQuery);
			}
		}
	}
}
