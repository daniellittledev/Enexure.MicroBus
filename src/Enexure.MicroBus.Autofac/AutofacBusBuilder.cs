using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus.Autofac
{
	public class AutofacBusBuilder : IBusBuilder
	{
		internal readonly ContainerBuilder containerBuilder;
		internal readonly List<MessageRegistration> registrations;

		public AutofacBusBuilder(ContainerBuilder containerBuilder)
		{
			this.containerBuilder = containerBuilder;
			this.registrations = new List<MessageRegistration>();
		}

		public ICommandBuilder<TCommand> RegisterCommand<TCommand>()
			where TCommand : ICommand
		{
			return new AutofacCommandBuilder<TCommand>(this);
		}

		public ICommandBuilder RegisterCommand(Type type)
		{
			return new AutofacCommandBuilder(this, type);
		}

		public IEventBuilder<TEvent> RegisterEvent<TEvent>()
			where TEvent : IEvent
		{
			return new AutofacEventBuilder<TEvent>(this);
		}

		public IEventBuilder RegisterEvent(Type type)
		{
			return new AutofacEventBuilder(this, type);
		}

		public IQueryBuilder<TQuery, TResult> RegisterQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return new AutofacQueryBuilder<TQuery, TResult>(this);
		}

		public IQueryBuilder RegisterQuery(Type type)
		{
			return new AutofacQueryBuilder(this, type);
		}

		public HandlerRegistar GetHandlerRegistar()
		{
			return new HandlerRegistar(registrations);
		}

		public IMicroBus BuildBus()
		{
			throw new NotImplementedException("The autofac bus builder does not implement BuildBus, use the extension method RegisterMicroBus for the autofac ContainerBuilder instead.");
		}
	}

	public class AutofacCommandBuilder : ICommandBuilder
	{
		private readonly AutofacBusBuilder busBuilder;
		private readonly Type commandType;

		public AutofacCommandBuilder(AutofacBusBuilder busBuilder, Type commandType)
		{
			this.busBuilder = busBuilder;
			this.commandType = commandType;
		}

		public IBusBuilder To(Type commandHandlerType, Pipeline pipeline)
		{
			var commandHandlerInterfaceType = typeof(ICommandHandler<>).MakeGenericType(commandType);
			busBuilder.registrations.Add(item: new MessageRegistration(commandType, commandHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType(commandHandlerType).As(commandHandlerInterfaceType).SingleInstance();

			return busBuilder;
		}
	}

	public class AutofacCommandBuilder<TCommand> : ICommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly AutofacBusBuilder busBuilder;

		public AutofacCommandBuilder(AutofacBusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var commandHandlerInterfaceType = typeof(ICommandHandler<TCommand>);
			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TCommand), commandHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType<TCommandHandler>().As<ICommandHandler<TCommand>>().SingleInstance();

			return busBuilder; 
		}
	}

	public class AutofacEventBuilder : IEventBuilder
	{
		private readonly AutofacBusBuilder busBuilder;
		private readonly Type eventType;

		public AutofacEventBuilder(AutofacBusBuilder busBuilder, Type eventType)
		{
			this.busBuilder = busBuilder;
			this.eventType = eventType;
		}

		public IBusBuilder To(Type eventHandlerType, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var eventHandlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(eventType);
			busBuilder.registrations.Add(item: new MessageRegistration(eventType, eventHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType(eventHandlerType).As(eventHandlerInterfaceType).SingleInstance();

			return busBuilder;
		}

		public IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (eventHandlerTypes == null) throw new ArgumentNullException("eventHandlerTypes");
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var eventHandlerTypesList = eventHandlerTypes.ToList();

			var eventHandlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(eventType);
			busBuilder.registrations.Add(item: new MessageRegistration(eventType, eventHandlerInterfaceType, pipeline));

			foreach (var eventHandlerType in eventHandlerTypesList) {
				busBuilder.containerBuilder.RegisterType(eventHandlerType).As(eventHandlerInterfaceType).SingleInstance();
			}

			return busBuilder;
		}
	}

	public class AutofacEventBuilder<TEvent> : IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly AutofacBusBuilder busBuilder;

		public AutofacEventBuilder(AutofacBusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var eventHandlerInterfaceType = typeof(IEventHandler<TEvent>);
			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), eventHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType<TEventHandler>().As<IEventHandler<TEvent>>().SingleInstance();

			return busBuilder;
		}

		public IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var binder = new AutofacEventBinder<TEvent>();
			eventBinder(binder);

			var eventHandlerTypesList = binder.GetHandlerTypes();

			var eventHandlerInterfaceType = typeof(IEventHandler<TEvent>);
			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), eventHandlerInterfaceType, pipeline));

			foreach (var eventHandlerType in eventHandlerTypesList) {
				busBuilder.containerBuilder.RegisterType(eventHandlerType).As<IEventHandler<TEvent>>().SingleInstance();
			}

			return busBuilder;
		}
	}

	public class AutofacEventBinder<TEvent> : IEventBinder<TEvent>
		where TEvent : IEvent
	{
		private readonly List<Type> eventTypes = new List<Type>();

		public IEventBinder<TEvent> Handler<THandler>()
			where THandler : IEventHandler<TEvent>
		{
			eventTypes.Add(typeof(THandler));

			return this;
		}

		internal IReadOnlyCollection<Type> GetHandlerTypes()
		{
			return eventTypes;
		}
	}

	public class AutofacQueryBuilder : IQueryBuilder
	{
		private readonly AutofacBusBuilder busBuilder;
		private readonly Type queryType;

		public AutofacQueryBuilder(AutofacBusBuilder busBuilder, Type queryType)
		{
			this.busBuilder = busBuilder;
			this.queryType = queryType;
		}

		public IBusBuilder To(Type queryHandlerType, Pipeline pipeline)
		{
			var queryHandlerInterfaceType = typeof(IQueryHandler<,>).MakeGenericType(queryType, queryType.GenericTypeArguments.Last());
			busBuilder.registrations.Add(item: new MessageRegistration(queryType, queryHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType(queryHandlerType).As(queryHandlerInterfaceType).SingleInstance();

			return busBuilder;
		}
	}

	public class AutofacQueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly AutofacBusBuilder busBuilder;

		public AutofacQueryBuilder(AutofacBusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var queryHandlerInterfaceType = typeof(IQueryHandler<TQuery, TResult>);
			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TQuery), queryHandlerInterfaceType, pipeline));
			busBuilder.containerBuilder.RegisterType<TQueryHandler>().As<IQueryHandler<TQuery, TResult>>().SingleInstance();

			return busBuilder;
		}
	}
}