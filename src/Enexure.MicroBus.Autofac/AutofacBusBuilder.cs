using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Enexure.MicroBus;
using Enexure.MicroBus.Autofac;

namespace Sample.Autofac
{
	public class AutofacBusBuilder
	{
		private readonly ContainerBuilder containerBuilder;
		readonly List<MessageRegistration> commandRegistrations = new List<MessageRegistration>();

		public AutofacBusBuilder(ContainerBuilder containerBuilder)
		{
			this.containerBuilder = containerBuilder;
		}

		public AutofacBusBuilder RegisterHandler<TCommandHandler>(AutofacPipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var commandType = typeof(TCommandHandler)
				.GetInterfaces()
				.First(x => x.IsGenericType
							&& x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
				.GenericTypeArguments
				.First();

			commandRegistrations.Add(item: new MessageRegistration(commandType, typeof(TCommandHandler), new Pipeline().AddHandlers(pipeline)));

			containerBuilder.RegisterType<TCommandHandler>().InstancePerLifetimeScope();

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