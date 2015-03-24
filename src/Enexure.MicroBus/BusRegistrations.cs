using System;
using System.Collections.Generic;
using System.Linq;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
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

			var innerCommandHandler = (ICommandHandler<TCommand>)Activator.CreateInstance(leafHandler);

			var handler = pipeline.Aggregate(
				(IPipelineHandler)new PretendToBePipelineHandler<TCommand>(innerCommandHandler),
				(current, handlerType) => (IPipelineHandler)Activator.CreateInstance(handlerType, current));

			return new PretendToBeCommandHandler<TCommand>(handler);
		}
	}
}