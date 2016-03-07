using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus.Annotations;
using Enexure.MicroBus.Messages;

namespace Enexure.MicroBus
{
	public class PipelineRunBuilder : IPipelineRunBuilder
	{
		private readonly IPipelineBuilder pipelineBuilder;
		private readonly IOuterPipelineDetertorUpdater updater;
		private readonly IDependencyScope dependencyScope;
		private readonly BusSettings busSettings;

		public PipelineRunBuilder(
			[NotNull]BusSettings busSettings,
			[NotNull]IPipelineBuilder pipelineBuilder,
			[NotNull]IOuterPipelineDetertorUpdater updater,
			[NotNull]IDependencyScope dependencyScope)
		{
			if (pipelineBuilder == null) throw new ArgumentNullException(nameof(pipelineBuilder));
			if (updater == null) throw new ArgumentNullException(nameof(updater));
			if (dependencyScope == null) throw new ArgumentNullException(nameof(dependencyScope));
			if (busSettings == null) throw new ArgumentNullException(nameof(busSettings));

			this.pipelineBuilder = pipelineBuilder;
			this.updater = updater;
			this.dependencyScope = dependencyScope;
			this.busSettings = busSettings;
		}

		public INextHandler GetRunnerForPipeline(Type messageType)
		{
			var pipeline = pipelineBuilder.GetPipeline(messageType);
			if (!pipeline.HandlerTypes.Any())
			{
				return NoHandlersForMessage(messageType);
			}

			return new NextHandlerRunner(message => BuildNextHandler(pipeline.DelegatingHandlerTypes, pipeline.HandlerTypes).Handle(message));
		}

		private INextHandler NoHandlersForMessage(Type messageType)
		{
			if (messageType == typeof(NoMatchingRegistrationEvent))
			{
				throw new NoRegistrationForMessageException(messageType);
			}

			try
			{
				var runner = GetRunnerForPipeline(typeof(NoMatchingRegistrationEvent));
				return new NextHandlerRunner(message => runner.Handle(new NoMatchingRegistrationEvent(message)));
			}
			catch (NoRegistrationForMessageException)
			{
				throw new NoRegistrationForMessageException(messageType);
			}
		}

		private INextHandler BuildNextHandler(
			IReadOnlyCollection<Type> delegatingHandlerTypes,
			IReadOnlyCollection<Type> handlerTypes
			)
		{
			return new NextHandlerRunner(async message => {

				if (message == null) {
					throw new NullMessageTypeException();
				}

				if (!delegatingHandlerTypes.Any()) {
					updater.PushMarker();
					var result = await RunHandlers(handlerTypes, message);
					updater.PopMarker();
					return result;
				}

				var head = delegatingHandlerTypes.First();
				var tail = delegatingHandlerTypes.Skip(1).ToList();

				var nextHandler = (IDelegatingHandler)dependencyScope.GetService(head);
				var nextFunction = BuildNextHandler(tail, handlerTypes);

				return await nextHandler.Handle(nextFunction, message);
			});
		}

		private async Task<object> RunHandlers(
			IReadOnlyCollection<Type> leftHandlerTypes,
			object message)
		{
			List<Task> tasks = new List<Task>();

			var handlers = leftHandlerTypes.Select(dependencyScope.GetService);

			if (busSettings.HandlerSynchronization == Synchronization.Syncronous) {

				foreach (var leafHandler in handlers) {
					Task task;
					await (task = ReflectionExtensions.CallHandleOnHandler(leafHandler, message));
					tasks.Add(task);
				}

			} else {
				tasks.AddRange(handlers.Select(handler => ReflectionExtensions.CallHandleOnHandler(handler, message)));
				await Task.WhenAll(tasks);
			}

			if (tasks.Count == 1) {
				return tasks.Select(ReflectionExtensions.GetTaskResult).Single();
			}

			return Unit.Unit;
		}
	}
}
