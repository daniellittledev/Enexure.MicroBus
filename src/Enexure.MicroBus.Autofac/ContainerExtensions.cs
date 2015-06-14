using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IBusBuilder, IBusBuilder> registerHandlers)
		{
			var builder = registerHandlers(new BusBuilder());

			RegisterHandlersWithAutofac(containerBuilder, builder);

			containerBuilder.RegisterInstance(builder.BuildHandlerRegistar()).As<IHandlerRegistar>().SingleInstance();
			containerBuilder.RegisterType<HandlerBuilder>().As<IHandlerBuilder>();
			containerBuilder.RegisterType<AutofacHandlerActivator>().As<IHandlerActivator>();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();

			return containerBuilder;
		}

		public static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, IBusBuilder builder)
		{
			var pipelines = new List<Pipeline>();

			foreach (var registration in builder.GetMessageRegistrations()) {

				if (!pipelines.Contains(registration.Pipeline)) {
					pipelines.Add(registration.Pipeline);
				}

				Type handlerInterface;
				switch (registration.Type) {
					case MessageType.Command:
						handlerInterface = typeof(ICommandHandler<>).MakeGenericType(registration.MessageType);
						break;
					case MessageType.Event:
						handlerInterface = typeof(IEventHandler<>).MakeGenericType(registration.MessageType);
						break;
					case MessageType.Query:
						var genericTypes = registration.MessageType.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuery<,>)).GenericTypeArguments;
						handlerInterface = typeof(IQueryHandler<,>).MakeGenericType(genericTypes);
						break;
					default:
						throw new NotImplementedException(string.Format("Could not build generic interface for message type {0}", registration.MessageType));
				}

				foreach (var handlerType in registration.Handlers) {
					containerBuilder.RegisterType(handlerType).As(handlerInterface).InstancePerLifetimeScope();
				}
			}

			var piplelineHandlers = pipelines.SelectMany(x => x).Distinct();
			foreach (var piplelineHandler in piplelineHandlers) {
				containerBuilder.RegisterType(piplelineHandler).As(piplelineHandler).InstancePerLifetimeScope();
			}
		}
	}
}
