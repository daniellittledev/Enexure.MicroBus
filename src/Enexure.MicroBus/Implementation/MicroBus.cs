using Enexure.MicroBus.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	using System.Threading;

	public class MicroBus : IMicroBus, ICancelableMicroBus
	{
		private readonly IDependencyResolver dependencyResolver;

		public MicroBus([NotNull]IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));

			this.dependencyResolver = dependencyResolver;
		}

		public Task SendAsync(ICommand busCommand)
		{
			return RunPipelineAsync(busCommand);
		}

		public Task PublishAsync(IEvent busEvent)
		{
			return RunPipelineAsync(busEvent);
		}

		public async Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> busQuery)
			where TQuery : IQuery<TQuery, TResult>
		{
			return (TResult)await RunPipelineAsync(busQuery);
		}

		public Task SendAsync(ICommand busCommand, CancellationToken cancellation)
		{
			return RunPipelineAsync(busCommand, cancellation);
		}

		public Task PublishAsync(IEvent busEvent, CancellationToken cancellation)
		{
			return RunPipelineAsync(busEvent, cancellation);
		}

		public async Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> busQuery, CancellationToken cancellation)
			where TQuery : IQuery<TQuery, TResult>
		{
			return (TResult)await RunPipelineAsync(busQuery, cancellation);
		}

		private async Task<object> RunPipelineAsync(
			object message,
			CancellationToken cancellation = default(CancellationToken))
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			using (var scope = dependencyResolver.BeginScope())
			{
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(message.GetType(), cancellation);
				return await messageProcessor.Handle(message);
			}
		}
	}
}
