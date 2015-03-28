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
			Register<TCommandHandler>(new Pipeline());
		}

		public void Register<TCommandHandler>(Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			//if (ICommandHandler<> typeof(TCommandHandler))
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
			return new MicroBus(new HandlerBuilder(new DefaultHandlerActivator(), new HandlerRegistar(commandRegistrations)));
		}
	}
}
