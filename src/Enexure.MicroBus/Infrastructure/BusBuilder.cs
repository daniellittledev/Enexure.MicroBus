using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		readonly List<MessageRegistration> registrations = new List<MessageRegistration>();

		public BusBuilder Register<THandler>()
		{
			Register<THandler>(new Pipeline());

			return this;
		}

		public BusBuilder Register<THandler>(Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var handlerType = typeof(THandler)
				.GetInterfaces()
				.FirstOrDefault(x => x.IsGenericType
					&& (x.GetGenericTypeDefinition() == typeof(ICommandHandler<>) 
						|| x.GetGenericTypeDefinition() == typeof(IEventHandler<>)
						|| x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

			if (handlerType == null) {
				throw new TypeIsNotAHandlerException();
			}

			var messageType = handlerType.GenericTypeArguments.First();

			registrations.Add(item: new MessageRegistration(messageType, typeof(THandler), pipeline));

			return this;
		}


		public IBus BuildBus()
		{
			return new MicroBus(new HandlerBuilder(new DefaultHandlerActivator(), new HandlerRegistar(registrations)));
		}
	}

	public class TypeIsNotAHandlerException : Exception
	{
	}
}
