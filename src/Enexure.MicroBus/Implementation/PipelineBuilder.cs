using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Enexure.MicroBus.Annotations;
using Enexure.MicroBus.Messages;
using Result = System.Object;

namespace Enexure.MicroBus
{
	public class PipelineBuilder : IPipelineBuilder
	{
		private readonly IHandlerProvider handlerProvider;
		private readonly IGlobalPipelineProvider globalPipelineProvider;
		private readonly IGlobalPipelineTracker tracker;
		private readonly IDependencyScope dependencyScope;
		private readonly BusSettings busSettings;

		public PipelineBuilder(
			[NotNull]BusSettings busSettings,
			[NotNull]IHandlerProvider handlerProvider,
			[NotNull]IGlobalPipelineProvider globalPipelineProvider,
			[NotNull]IGlobalPipelineTracker tracker,
			[NotNull]IDependencyScope dependencyScope)
		{
			if (handlerProvider == null) throw new ArgumentNullException(nameof(handlerProvider));
			if (globalPipelineProvider == null) throw new ArgumentNullException(nameof(globalPipelineProvider));
			if (tracker == null) throw new ArgumentNullException(nameof(tracker));
			if (dependencyScope == null) throw new ArgumentNullException(nameof(dependencyScope));
			if (busSettings == null) throw new ArgumentNullException(nameof(busSettings));

			this.handlerProvider = handlerProvider;
			this.globalPipelineProvider = globalPipelineProvider;
			this.tracker = tracker;
			this.dependencyScope = dependencyScope;
			this.busSettings = busSettings;
		}

		public Func<IMessage, Task<Result>> GetPipelineForMessage(Type messageType)
		{
			GroupedMessageRegistration registration;
			if (!handlerProvider.GetRegistrationForMessage(messageType, out registration)) {
				if (messageType == typeof(NoMatchingRegistrationEvent)) {
					throw new NoRegistrationForMessageException(messageType);
				}

				try {
					var runner = GetPipelineForMessage(typeof(NoMatchingRegistrationEvent));

					return message => {
						return runner(new NoMatchingRegistrationEvent(message));
					};

				} catch (NoRegistrationForMessageException) {
					throw new NoRegistrationForMessageException(messageType);
				}
			}

			var completePipeline = tracker.HasRun 
				? registration.Pipeline.ToList() 
				: globalPipelineProvider.GetGlobalPipeline().Concat(registration.Pipeline).ToList();

			tracker.MarkAsRun();

			return message => GenerateNext(busSettings, dependencyScope, completePipeline, registration.Handlers)(message);
		}

		private static Func<IMessage, Task<object>> GenerateNext(
			BusSettings busSettings,
			IDependencyScope dependencyScope,
			IReadOnlyCollection<Type> pipelineHandlerTypes,
			IEnumerable<Type> leftHandlerTypes
			)
		{
			return (async message => {

				if (message == null) {
					throw new NullMessageTypeException();
				}

				if (!pipelineHandlerTypes.Any()) {
					return await RunLeafHandlers(busSettings, dependencyScope, leftHandlerTypes, message);
				}

				var head = pipelineHandlerTypes.First();
				var nextHandler = (IPipelineHandler)dependencyScope.GetService(head);

				var tail = pipelineHandlerTypes.Skip(1).ToList();
				var nextFunction = GenerateNext(busSettings, dependencyScope, tail, leftHandlerTypes);

				return await nextHandler.Handle(nextFunction, message);
			});
		}

		private static async Task<Result> RunLeafHandlers(
			BusSettings busSettings,
			IDependencyScope dependencyScope,
			IEnumerable<Type> leftHandlerTypes,
			IMessage message)
		{
			Task lastTask = null;
			var handlers = leftHandlerTypes.Select(dependencyScope.GetService);

			if (busSettings.DisableParallelHandlers) {
				foreach (var leafHandler in handlers) {
					await (lastTask = CallHandleOnHandler(leafHandler, message));
				}
			} else {
				await Task.WhenAll(handlers.Select(handler => (lastTask = CallHandleOnHandler(handler, message))));
			}

			if (lastTask == null) {
				throw new NullReferenceException("Sanity Check fail: while running leaf handlers the last task was null, but an instance was expected.");
			}

			return GetTaskResult(lastTask);
		}

		private static object GetTaskResult(Task task)
		{
			var taskType = task.GetType();
			var typeInfo = taskType.GetTypeInfo();
			if (!typeInfo.IsGenericType) { 
				throw new SomehowRecievedTaskWithoutResultException();
			}

			var resultProperty = typeInfo.GetDeclaredProperty("Result").GetMethod;
			return resultProperty.Invoke(task, new object[] { });
		}

		private static Task CallHandleOnHandler(object handler, IMessage message)
		{
            var type = handler.GetType();
			var messageType = message.GetType();

			//type.GetDeclaredMethods("Handle", BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new[] {messageType}, null);
			var handleMethods = type.GetRuntimeMethods().Where(m => m.Name == "Handle");
			var handleMethod = handleMethods.Single(x => {
				var parameterTypeIsCorrect = x.GetParameters().Single().ParameterType.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo());
				return parameterTypeIsCorrect 
					&& x.IsPublic 
					&& ((x.CallingConvention & CallingConventions.HasThis) != 0);
				});

			var objectTask = handleMethod.Invoke(handler, new object[] {message});

			if (objectTask == null) {
				throw new NullReferenceException(string.Format("Handler for message of type '{0}' returned null.{1}To Resolve you can try{1} 1) Return a task instead", messageType, Environment.NewLine));
			}

			return (Task)objectTask;
		}
	}

}
