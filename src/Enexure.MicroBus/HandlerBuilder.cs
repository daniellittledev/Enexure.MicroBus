using System;
using System.Linq;
using Enexure.MicroBus.InfrastructureContracts;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class HandlerBuilder : IHandlerBuilder
	{
		private readonly IHandlerActivator handlerActivator;
		private readonly IHandlerRegistar handlerRegistar;

		public HandlerBuilder(IHandlerActivator handlerActivator, IHandlerRegistar handlerRegistar)
		{
			this.handlerActivator = handlerActivator;
			this.handlerRegistar = handlerRegistar;
		}

		public ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand
		{
			return GetRunnerForMessage<ICommandHandler<TCommand>, TCommand>(
				handler => new CommandHandlerPretendToBePipelineHandler<TCommand>(handler),
				handler => new PretendToBeCommandHandler<TCommand>(handler));
		}

		public IEventHandler<TEvent> GetRunnerForEvent<TEvent>() 
			where TEvent : IEvent
		{
			return GetRunnerForMessage<IEventHandler<TEvent>, TEvent>(
				handler => new EventHandlerPretendToBePipelineHandler<TEvent>(handler),
				handler => new PretendToBeEventHandler<TEvent>(handler));
		}

		public IQueryHandler<TQuery, TResult> GetRunnerForQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return GetRunnerForMessage<IQueryHandler<TQuery, TResult>, TQuery>(
				handler => new QueryHandlerPretendToBePipelineHandler<TQuery, TResult>(handler), 
				handler => new PretendToBeQueryHandler<TQuery, TResult>(handler));
		}

		private THandler GetRunnerForMessage<THandler, TMessage>(Func<THandler, IPipelineHandler> makePretend, Func<IPipelineHandler, THandler> makeReal)
			where TMessage : IMessage
		{
			var registration = handlerRegistar.GetRegistrationForMessage(typeof(TMessage));

			var innerEventHandler = handlerActivator.ActivateHandler<THandler>(registration.MessageHandlerType);

			//var multiHandler = 

			var handler = registration.Pipeline.Aggregate(
				makePretend(innerEventHandler),
				(current, handlerType) => handlerActivator.ActivateHandler<IPipelineHandler>(handlerType, current));

			return makeReal(handler);
		}
	}
}