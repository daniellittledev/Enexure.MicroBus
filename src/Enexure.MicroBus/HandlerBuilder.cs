using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus.BuiltInEvents;

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

		public Func<TCommand, Task> GetRunnerForCommand<TCommand>(IDependencyScope scope)
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

		private Func<TMessage, Task<object>> GetRunnerForMessage<THandler, TMessage>(
			IDependencyScope scope,
			Func<TMessage, IEnumerable<THandler>, Task<object>> runHandlers
			)
			where TMessage : IMessage
		{
			var messageType = typeof(TMessage);
			var registration = handlerRegistar.GetRegistrationForMessage(messageType);

			if (registration == null) {
				if (typeof(TMessage) == typeof(NoMatchingRegistrationEvent)) {
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

			return message => GenerateNext(scope, registration.Pipeline.ToList(), registration.Handlers, runHandlers)(message);
		}

		Func<IMessage, Task<object>> GenerateNext<THandler, TMessage>(
			IDependencyScope scope, 
			IReadOnlyCollection<Type> pipelineHandlerTypes, 
			IEnumerable<Type> handlerTypes,
			Func<TMessage, IEnumerable<THandler>, Task<object>> runHandlers)
			where TMessage : IMessage
		{
			return (message => {

				if (message == null) {
					throw new NullMessageTypeException(typeof(TMessage));

				} else if (!(message is TMessage)) {
					throw new InvalidMessageTypeException(message.GetType(), typeof(TMessage));
				}

				if (!pipelineHandlerTypes.Any()) {
					return runHandlers((TMessage)message, handlerTypes.Select(scope.GetService).Cast<THandler>());
				}

				var head = (IPipelineHandler)scope.GetService(pipelineHandlerTypes.First());
				var next = GenerateNext(scope, pipelineHandlerTypes.Skip(1).ToList(), handlerTypes, runHandlers);

				return head.Handle(next, message);

			});

		}

	}

	public class NullMessageTypeException : Exception
	{
		public NullMessageTypeException(Type type)
			: base(string.Format("Message was null but an instance of type '{0}' was expected", type.Name))
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
