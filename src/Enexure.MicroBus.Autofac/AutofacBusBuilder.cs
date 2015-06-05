using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Enexure.MicroBus.InfrastructureContracts;

namespace Enexure.MicroBus.Autofac
{
	public class AutofacBusBuilder
	{
		private readonly ContainerBuilder containerBuilder;
		readonly List<MessageRegistration> commandRegistrations = new List<MessageRegistration>();

		public AutofacBusBuilder(ContainerBuilder containerBuilder)
		{
			this.containerBuilder = containerBuilder;
		}

		public AutofacBusBuilder RegisterHandler<THandler>(AutofacPipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var messageType = typeof(THandler)
				.GetInterfaces()
				.First(x => x.IsGenericType
							&& (x.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
							|| x.GetGenericTypeDefinition() == typeof(IEventHandler<>)
							|| x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
				.GenericTypeArguments
				.First();

			commandRegistrations.Add(item: new MessageRegistration(messageType, typeof(THandler), new Pipeline().AddHandlers(pipeline)));

			containerBuilder.RegisterType<THandler>().InstancePerLifetimeScope();

			return this;
		}

		public HandlerRegistar Build()
		{
			return new HandlerRegistar(commandRegistrations);
		}

		public AutofacPipeline CreatePipeline()
		{
			return new AutofacPipeline(containerBuilder);
		}
	}
}