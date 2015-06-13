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

		public IMicroBus BuildBus()
		{
			return new MicroBus(new HandlerBuilder(new DefaultHandlerActivator(), new HandlerRegistar(registrations)));
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
			busBuilder.registrations.Add(item: new MessageRegistration(commandType, commandHandlerType, pipeline));
			busBuilder.containerBuilder.RegisterType(commandHandlerType).As(typeof(ICommandHandler<>).MakeGenericType(commandType)).SingleInstance();

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

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline));
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

			busBuilder.registrations.Add(item: new Enexure.MicroBus.MessageRegistration(eventType, eventHandlerType, pipeline));
			busBuilder.containerBuilder.RegisterType(eventHandlerType).As(typeof(IEventHandler<>).MakeGenericType(eventType)).SingleInstance();

			return busBuilder;
		}

		public IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (eventHandlerTypes == null) throw new ArgumentNullException("eventHandlerTypes");
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var eventHandlerTypesList = eventHandlerTypes.ToList();

			busBuilder.registrations.Add(item: new MessageRegistration(eventType, eventHandlerTypesList, pipeline));

			foreach (var eventHandlerType in eventHandlerTypesList) {
				busBuilder.containerBuilder.RegisterType(eventHandlerType).As(typeof(IEventHandler<>).MakeGenericType(eventType)).SingleInstance();
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

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), typeof(TEventHandler), pipeline));
			busBuilder.containerBuilder.RegisterType<TEventHandler>().As<IEventHandler<TEvent>>().SingleInstance();

			return busBuilder;
		}

		public IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var binder = new AutofacEventBinder<TEvent>();
			eventBinder(binder);

			var eventHandlerTypesList = binder.GetHandlerTypes();

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), eventHandlerTypesList, pipeline));

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
			busBuilder.registrations.Add(item: new MessageRegistration(queryType, queryHandlerType, pipeline));
			busBuilder.containerBuilder.RegisterType(queryHandlerType).As(typeof(IQueryHandler<,>).MakeGenericType(queryType, queryType.GenericTypeArguments.Last())).SingleInstance();

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

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));
			busBuilder.containerBuilder.RegisterType<TQueryHandler>().As<IQueryHandler<TQuery, TResult>>().SingleInstance();

			return busBuilder;
		}
	}
}