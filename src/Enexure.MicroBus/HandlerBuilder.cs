using System;
using System.Collections.Generic;
using System.Linq;
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

		public Func<IMessage, Task> GetRunnerForCommand<TCommand>(IDependencyScope scope)
			where TCommand : ICommand
		{
			return GetRunnerForMessage<ICommandHandler<TCommand>, TCommand>(scope,
				async (message, handlers) => {
					if (busSettings.DisableParallelHandlers) {
						foreach (var handler in handlers) {
							await handler.Handle(message);
						}
					} else {
						await Task.WhenAll(handlers.Select(x => x.Handle(message)));
					}
					return null;
				});
		}

		public Func<TEvent, Task> GetRunnerForEvent<TEvent>(IDependencyScope scope) 
			where TEvent : IEvent
		{
			return GetRunnerForMessage<IEventHandler<TEvent>, TEvent>(scope,
				async (message, handlers) => {
					if (busSettings.DisableParallelHandlers) {
						foreach (var handler in handlers) {
							await handler.Handle(message);
						}
					} else {
						await Task.WhenAll(handlers.Select(x => x.Handle(message)));
					}
					return null;
				});
		}

		public Func<TQuery, Task<TResult>> GetRunnerForQuery<TQuery, TResult>(IDependencyScope scope)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return async message => (TResult) await GetRunnerForMessage<IQueryHandler<TQuery, TResult>, TQuery>(scope,
				async (msg, handlers) => await handlers.Single().Handle(msg))(message);
		}

		private Func<IMessage, Task<object>> GetRunnerForMessage(IDependencyScope scope, Type messageType)
		{
			var registration = handlerRegistar.GetRegistrationForMessage(messageType);

			if (registration == null) {
				if (messageType == typeof(NoMatchingRegistrationEvent)) {
					throw new NoRegistrationForMessageException(messageType);
				}

				Func<NoMatchingRegistrationEvent, Task> runner;
				try {
					runner = GetRunnerForEvent<NoMatchingRegistrationEvent>(scope);

				} catch (NoRegistrationForMessageException) {
					throw new NoRegistrationForMessageException(messageType);
				}

				return message => {
					runner(new NoMatchingRegistrationEvent(message));
					throw new NoRegistrationForMessageException(messageType);
				};
			}

			return message => GenerateNext(scope, registration.Pipeline.ToList(), registration.Handlers)(message);
		}

		Func<IMessage, Task<object>> GenerateNext(
			IDependencyScope scope, 
			IReadOnlyCollection<Type> pipelineHandlerTypes,
			IEnumerable<Type> leftHandlerTypes
			)
		{
			return (message => {

				if (message == null) {
					throw new NullMessageTypeException();
				}

				if (!pipelineHandlerTypes.Any()) {

// ReSharper disable once PossibleMultipleEnumeration
					foreach (dynamic leafHandler in leftHandlerTypes.Select(scope.GetService)) {
						leafHandler.Handle(message);
					}
				}

				var head = pipelineHandlerTypes.First();
				var tail = pipelineHandlerTypes.Skip(1).ToList();

				var nextHandler = (IPipelineHandler)scope.GetService(head);

// ReSharper disable once PossibleMultipleEnumeration
				var nextFunction = GenerateNext(scope, tail, leftHandlerTypes);

				return nextHandler.Handle(nextFunction, message);

			});

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
