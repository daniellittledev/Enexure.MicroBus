using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Enexure.MicroBus.Messages;

namespace Enexure.MicroBus
{
    using System.Threading;

    public class PipelineRunBuilder : IPipelineRunBuilder
    {
        private readonly IPipelineBuilder pipelineBuilder;
        private readonly IOuterPipelineDetertorUpdater updater;
        private readonly IDependencyScope dependencyScope;
        private readonly BusSettings busSettings;

        public PipelineRunBuilder(
            BusSettings busSettings,
            IPipelineBuilder pipelineBuilder,
            IOuterPipelineDetertorUpdater updater,
            IDependencyScope dependencyScope)
        {
            this.pipelineBuilder = pipelineBuilder ?? throw new ArgumentNullException(nameof(pipelineBuilder));
            this.updater = updater ?? throw new ArgumentNullException(nameof(updater));
            this.dependencyScope = dependencyScope ?? throw new ArgumentNullException(nameof(dependencyScope));
            this.busSettings = busSettings ?? throw new ArgumentNullException(nameof(busSettings));
        }

        public INextHandler GetRunnerForPipeline(Type messageType, CancellationToken cancellation)
        {
            if (cancellation == null) throw new ArgumentNullException(nameof(cancellation));

            var pipeline = pipelineBuilder.GetPipeline(messageType);
            if (!pipeline.HandlerTypes.Any())
            {
                return NoHandlersForMessage(messageType, cancellation);
            }

            return new NextHandlerRunner(message => {
                var firstHandler = BuildNextHandler(pipeline.DelegatingHandlerTypes, pipeline.HandlerTypes, cancellation);
                return firstHandler.Handle(message);
            });
        }

        private INextHandler NoHandlersForMessage(Type messageType, CancellationToken cancellation)
        {
            if (messageType == typeof(NoMatchingRegistrationEvent))
            {
                throw new NoRegistrationForMessageException(messageType);
            }

            try
            {
                var runner = GetRunnerForPipeline(typeof(NoMatchingRegistrationEvent), cancellation);
                return new NextHandlerRunner(message => runner.Handle(new NoMatchingRegistrationEvent(message)));
            }
            catch (NoRegistrationForMessageException)
            {
                throw new NoRegistrationForMessageException(messageType);
            }
        }

        private INextHandler BuildNextHandler(
            IReadOnlyCollection<Type> delegatingHandlerTypes,
            IReadOnlyCollection<Type> handlerTypes,
            CancellationToken cancellation)
        {
            return new NextHandlerRunner(async message => {

                if (message == null) {
                    throw new NullMessageTypeException();
                }

                if (!delegatingHandlerTypes.Any()) {
                    updater.PushMarker();
                    var result = await RunHandlers(handlerTypes, message, cancellation);
                    updater.PopMarker();
                    return result;
                }

                var head = delegatingHandlerTypes.First();
                var tail = delegatingHandlerTypes.Skip(1).ToList();

                var nextFunction = BuildNextHandler(tail, handlerTypes, cancellation);

                var pipelineHanlder = dependencyScope.GetService(head);
                if (pipelineHanlder is IDelegatingHandler nextDelegatingHandler) {

                    return await nextDelegatingHandler.Handle(nextFunction, message);

                } else if (pipelineHanlder is ICancelableDelegatingHandler nextCancelableDelegatingHandler) {

                    return await nextCancelableDelegatingHandler.Handle(nextFunction, message, cancellation);

                } else {
                    
                    throw new AskedForDelegatingHandlerButDidNotGetADelegatingHandlerException();
                }
            });
        }

        private async Task<object> RunHandlers(
            IReadOnlyCollection<Type> leafHandlerTypes,
            object message,
            CancellationToken cancellation)
        {
            var handlers = leafHandlerTypes.Select(dependencyScope.GetService);

            var tasks = handlers.Select(handler => ReflectionExtensions.CallHandleOnHandler(handler, message, cancellation));

            var taskList = await RunTasks(tasks, busSettings.HandlerSynchronization);

            if (taskList.Count == 1)
            {
                var task = taskList.Single();
                // Auto registered commands and events may return a Task with no result
                if (task.GetType().GetTypeInfo().IsGenericType) {
                    return ReflectionExtensions.GetTaskResult(task);
                }
            }

            return Unit.Unit;
        }

        private async Task<IReadOnlyCollection<Task>> RunTasks(IEnumerable<Task> tasks, Synchronization synchronization)
        {
            var taskList = new List<Task>();
            if (synchronization == Synchronization.Syncronous)
            {
                foreach (var task in tasks)
                {
                    taskList.Add(task);
                    await task;
                }
            }
            else
            {
                taskList.AddRange(tasks);
                await Task.WhenAll(taskList);
            }
            return taskList;
        }
    }
}
