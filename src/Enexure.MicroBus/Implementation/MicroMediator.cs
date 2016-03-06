using Enexure.MicroBus.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public class MicroMediator : IMicroMediator
	{
		private readonly IDependencyResolver dependencyResolver;

		public MicroMediator([NotNull]IDependencyResolver dependencyResolver)
		{
			if (dependencyResolver == null) throw new ArgumentNullException(nameof(dependencyResolver));

			this.dependencyResolver = dependencyResolver;
		}

		public async Task SendAsync(object message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			using (var scope = dependencyResolver.BeginScope()) {
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(message.GetType());
				(await messageProcessor.Handle(message)).Single();
			}
		}

		public async Task PublishAsync(object message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			using (var scope = dependencyResolver.BeginScope())
			{
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(message.GetType());
				await messageProcessor.Handle(message);
			}
		}

		public async Task<TResult> QueryAsync<TResult>(object message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			using (var scope = dependencyResolver.BeginScope())
			{
				var builder = scope.GetService<IPipelineRunBuilder>();
				var messageProcessor = builder.GetRunnerForPipeline(message.GetType());
				return (await messageProcessor.Handle(message)).Cast<TResult>().Single();
			}
		}
	}
}
