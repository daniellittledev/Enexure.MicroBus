using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class MicroBus : IBus
	{
		private readonly IBusRegistrations registrations;

		public MicroBus(IBusRegistrations registrations)
		{
			this.registrations = registrations;
		}

		public Task Send<TCommand>(TCommand busCommand)
			where TCommand : ICommand
		{
			var handler = registrations.GetRunnerForCommand<TCommand>();
			return handler.Handle(busCommand);
		}

		public Task Publish<TEvent>(TEvent @event)
			where TEvent : IEvent 
		{
			throw new NotImplementedException();
		}

		public Task<TResult> Query<TQuery, TResult>(TQuery query)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			throw new NotImplementedException();
		}
	}

	public interface IBusRegistrations
	{
		ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand;
	}

	public class BusRegistrations : IBusRegistrations
	{
		private readonly IDictionary<Type, CommandRegistration> commandRegistrationsLookup;

		public BusRegistrations(IEnumerable<CommandRegistration> commandRegistrations)
		{
			commandRegistrationsLookup = commandRegistrations.ToDictionary(x => x.CommandType, x => x);
		}

		public ICommandHandler<TCommand> GetRunnerForCommand<TCommand>()
			where TCommand : ICommand
		{
			var registration = commandRegistrationsLookup[typeof(TCommand)];

			var pipeline = registration.Pipeline;
			var leafHandler = registration.CommandHandlerType;

			var handler = (IPipelineHandler)pipeline.Aggregate(
				Activator.CreateInstance(leafHandler), 
				(current, handlerType) => Activator.CreateInstance(handlerType, current));

			//(ICommandHandler<TCommand>)
			//return handler;

			throw new NotImplementedException();
		}
	}

	public class CommandRegistration
	{
		private readonly Type commandType;
		private readonly Type commandHandlerType;
		private readonly Pipeline pipeline;

		public CommandRegistration(Type commandType, Type commandHandlerType, Pipeline pipeline)
		{
			this.commandType = commandType;
			this.commandHandlerType = commandHandlerType;
			this.pipeline = pipeline;
		}

		public Type CommandType
		{
			get { return commandType; }
		}

		public Type CommandHandlerType
		{
			get { return commandHandlerType; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}
	}
}
