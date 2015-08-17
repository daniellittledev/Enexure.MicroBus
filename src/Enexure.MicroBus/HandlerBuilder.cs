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

		Func<IMessage, Task<object>> GenerateNext(
			IDependencyScope scope, 
			IReadOnlyCollection<Type> pipelineHandlerTypes,
			IEnumerable<Type> leftHandlerTypes
			)
		{
			return (async message => {

				if (message == null) {
					throw new NullMessageTypeException();
				}

				if (!pipelineHandlerTypes.Any())
				{
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
				// ReSharper disable once PossibleMultipleEnumeration
				foreach (object leafHandler in handlers) {
					Task task = CallHandleOnHandler(leafHandler, message);
					lastTask = task;
					await task;
				}
			} else {
				await Task.WhenAll(handlers.Select(handler =>
				{
				    Task task = CallHandleOnHandler(handler, message);
					lastTask = task;
					return task;
				}));
			}

		    if (lastTask == null)
		    {
                            
		    }

		    if (lastTask != null && lastTask.GetType().IsGenericType)
			{

                dynamic taskWithResult = lastTask;
			    try
			    {
                    object result = taskWithResult.Result;
                    return result;
                }
			    catch (Exception)
			    {
			        
			        throw;
			    }
				
			    
			} else {
				return null;
			}
		}

        private Task CallHandleOnHandler(object handler, IMessage message)
        {
            var type = handler.GetType();
            var messageType = message.GetType();

            var handleMethod = type.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new [] { messageType }, null);

            var objectTask = handleMethod.Invoke(handler, new object[] { message });

            if (objectTask == null) {
                throw new NullReferenceException(string.Format("Handler for message of type '{0}' returned null.{1}To Resolve you can try{1} 1) Return a task instead", messageType, ));
            }

            return (Task) objectTask;
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
