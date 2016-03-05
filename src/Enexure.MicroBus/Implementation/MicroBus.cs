using Enexure.MicroBus.Annotations;
using System;
using System.Linq;
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

		public async Task SendAsync(ICommand busCommand)
		{
			if (busCommand == null) throw new ArgumentNullException(nameof(busCommand));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(busCommand.GetType());
				await messageProcessor.Handle(busCommand);
			}
		}

		public async Task PublishAsync(IEvent busEvent)
		{
			if (busEvent == null) throw new ArgumentNullException(nameof(busEvent));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(busEvent.GetType());
				await messageProcessor.Handle(busEvent);
			}
		}

		public async Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> busQuery)
			where TQuery : IQuery<TQuery, TResult>
		{
			if (busQuery == null) throw new ArgumentNullException(nameof(busQuery));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(busQuery.GetType());
				return (TResult) (await messageProcessor.Handle(busQuery)).Single();
			}
		}
	}
}
