using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Enexure.MicroBus.BuiltInEvents;

using Handler = System.Object;
using Result = System.Object;

namespace Enexure.MicroBus
{
	public class HandlerBuilder : IHandlerBuilder
	{
		private readonly IHandlerRegistar handlerRegistar;
		private readonly BusSettings busSettings;

		public HandlerBuilder(IHandlerRegistar handlerRegistar, BusSettings busSettings)
		{
			this.handlerRegistar = handlerRegistar;
			this.busSettings = busSettings;
		}

		public Func<IMessage, Task<Result>> GetRunnerForMessage(IDependencyScope scope, Type messageType)
		{
			var registration = handlerRegistar.GetRegistrationForMessage(messageType);

			if (registration == null) {
				if (messageType == typeof(NoMatchingRegistrationEvent)) {
					throw new NoRegistrationForMessageException(messageType);
				}

				try {
					var runner = GetRunnerForMessage(scope, typeof(NoMatchingRegistrationEvent));

					return message => {
						runner(new NoMatchingRegistrationEvent(message));
						throw new NoRegistrationForMessageException(messageType);
					};

				} catch (NoRegistrationForMessageException) {
					throw new NoRegistrationForMessageException(messageType);
				}
			}

			return message => GenerateNext(scope, registration.Pipeline.ToList(), registration.Handlers)(message);
		}

		private Func<IMessage, Task<object>> GenerateNext(
			IDependencyScope scope,
			IReadOnlyCollection<Type> pipelineHandlerTypes,
			IEnumerable<Type> leftHandlerTypes
			)
		{
			return (async message => {

				if (message == null) {
					throw new NullMessageTypeException();
				}

				if (!pipelineHandlerTypes.Any()) {
					return await RunLeafHandlers(scope, leftHandlerTypes, message);
				}

				var head = pipelineHandlerTypes.First();
				var tail = pipelineHandlerTypes.Skip(1).ToList();

				var nextHandler = (IPipelineHandler)scope.GetService(head);

				var nextFunction = GenerateNext(scope, tail, leftHandlerTypes);

				return await nextHandler.Handle(nextFunction, message);

			});

		}

		private async Task<Result> RunLeafHandlers(IDependencyScope scope, IEnumerable<Type> leftHandlerTypes, IMessage message)
		{
			Task lastTask = null;
			var handlers = leftHandlerTypes.Select(scope.GetService);
			if (busSettings.DisableParallelHandlers) {
				foreach (var leafHandler in handlers) {
					var task = CallHandleOnHandler(leafHandler, message);
					await (lastTask = task);
				}
			} else {
				await Task.WhenAll(handlers.Select(handler => {
					var task = CallHandleOnHandler(handler, message);
					return (lastTask = task);
				}));
			}

			if (lastTask == null) {
				throw new NullReferenceException("Sanity Check fail: while running leaf handlers the last task was null, but an instance was expected.");
			}

			var taskType = lastTask.GetType();
			if (!taskType.IsGenericType) {
				throw new SomehowRecievedTaskWithoutResultException();
			}

			var resultProperty = taskType.GetProperty("Result").GetMethod;
			return resultProperty.Invoke(lastTask, new object[] {});
		}

		private Task CallHandleOnHandler(object handler, IMessage message)
		{
			var type = handler.GetType();
			var messageType = message.GetType();

			var handleMethod = type.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new[] {messageType}, null);

			var objectTask = handleMethod.Invoke(handler, new object[] {message});

			if (objectTask == null) {
				throw new NullReferenceException(string.Format("Handler for message of type '{0}' returned null.{1}To Resolve you can try{1} 1) Return a task instead", messageType, Environment.NewLine));
			}

			return (Task)objectTask;
		}
	}

	public class SomehowRecievedTaskWithoutResultException : Exception
	{
		public SomehowRecievedTaskWithoutResultException()
			: base(string.Format("Tasks returned by handlers should return Task<?> but Task was returned instead, this is impossible"))
		{
		}
	}

	public class NullMessageTypeException : Exception
	{
		public NullMessageTypeException(Type type)
			: base(string.Format("Message was null but an instance of type '{0}' was expected", type.Name))
		{
		}

		public NullMessageTypeException()
			: base("Message was null")
		{
		}
	}

	public class InvalidMessageTypeException : Exception
	{
		public InvalidMessageTypeException(Type getType, Type type)
			: base(string.Format("Message was of type '{0}' but an instance of type '{1}' was expected", getType.Name, type.Name))
		{
		}
	}

	public class NoRegistrationForMessageException : Exception
	{
		public NoRegistrationForMessageException(Type commandType)
			: base(string.Format("No registration for message of type {0} was found", commandType.Name))
		{
		}
	}
}
