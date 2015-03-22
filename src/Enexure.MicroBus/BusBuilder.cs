using System;
using System.Collections.Generic;
using System.Linq;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		readonly List<CommandRegistration> commandRegistrations = new List<CommandRegistration>();

		public void Register<TCommandHandler>()
		{
		
		}

		public void Register<TCommandHandler>(Pipeline pipeline)
		{
			// Check type matches ICommandHandler

			var commandType = typeof(TCommandHandler)
				.GetInterfaces()
				.First(x => x.IsGenericType
				         && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
				.GenericTypeArguments
				.First();

			commandRegistrations.Add(item: new CommandRegistration(commandType, typeof(TCommandHandler), pipeline));
		}

		public IBus BuildBus()
		{
			return new MicroBus(new BusRegistrations(commandRegistrations));
		}
	}
}
