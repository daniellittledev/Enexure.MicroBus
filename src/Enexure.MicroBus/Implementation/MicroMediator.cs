using Enexure.MicroBus.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	using System.Threading;

	public class MicroMediator : IMicroMediator, ICancelableMicroMediator
	{
		private readonly IDependencyResolver dependencyResolver;

		public MicroMediator([NotNull]IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));

			this.dependencyResolver = dependencyResolver;
		}

		public Task SendAsync(object message)
		{
			return RunPipelineAsync(message);
		}

		public Task PublishAsync(object message)
		{
			return RunPipelineAsync(message);
		}

		public async Task<TResult> QueryAsync<TResult>(object message)
		{
			return (TResult)await RunPipelineAsync(message);
		}

		public Task SendAsync(object message, CancellationToken cancellation)
		{
			return RunPipelineAsync(message, cancellation);
		}

		public Task PublishAsync(object message, CancellationToken cancellation)
		{
			return RunPipelineAsync(message, cancellation);
		}

		public async Task<TResult> QueryAsync<TResult>(object message, CancellationToken cancellation)
		{
			return (TResult)await RunPipelineAsync(message, cancellation);
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
