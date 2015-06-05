using System;
using System.Collections.Generic;
using System.Linq;
using Enexure.MicroBus.InfrastructureContracts;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		internal readonly List<MessageRegistration> registrations = new List<MessageRegistration>();

		public CommandBuilder<TCommand> RegisterCommand<TCommand>()
			where TCommand : ICommand
		{
			return new CommandBuilder<TCommand>(this);
		}

		public CommandBuilder RegisterCommand(Type type)
		{
			return new CommandBuilder(this, type);
		}

		public EventBuilder<TEvent> RegisterEvent<TEvent>()
			where TEvent : IEvent
		{
			return new EventBuilder<TEvent>(this);
		}

		public EventBuilder RegisterEvent(Type type)
		{
			return new EventBuilder(this, type);
		}

		public QueryBuilder<TQuery, TResult> RegisterQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return new QueryBuilder<TQuery, TResult>(this);
		}

		public QueryBuilder RegisterQuery(Type type)
		{
			return new QueryBuilder(this, type);
		}

		public IMicroBus BuildBus()
		{
			return new MicroBus(new HandlerBuilder(new DefaultHandlerActivator(), new HandlerRegistar(registrations)));
		}
	}

	public class CommandBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type commandType;

		public CommandBuilder(BusBuilder busBuilder, Type commandType)
		{
			this.busBuilder = busBuilder;
			this.commandType = commandType;
		}

		public BusBuilder To(Type commandHandlerType, Pipeline pipeline)
		{
			busBuilder.registrations.Add(item: new MessageRegistration(commandType, commandHandlerType, pipeline));

			return busBuilder;
		}
	}

	public class CommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly BusBuilder busBuilder;

		public CommandBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public BusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline));

			return busBuilder; 
		}
	}

	public class EventBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type eventType;

		public EventBuilder(BusBuilder busBuilder, Type eventType)
		{
			this.busBuilder = busBuilder;
			this.eventType = eventType;
		}

		public BusBuilder To(Type eventHandlerType, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			busBuilder.registrations.Add(item: new MessageRegistration(eventType, eventHandlerType, pipeline));

			return busBuilder;
		}

		public BusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			busBuilder.registrations.Add(item: new MessageRegistration(eventType, eventHandlerTypes, pipeline));

			return busBuilder;
		}
	}

	public class EventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly BusBuilder busBuilder;

		public EventBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public BusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), typeof(TEventHandler), pipeline));

			return busBuilder;
		}

		public BusBuilder To<TEventHandler>(Action<EventBinder<TEventHandler>> eventBinder, Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var binder = new EventBinder<TEventHandler>();
			eventBinder(binder);

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TEvent), binder.GetHandlerTypes(), pipeline));

			return busBuilder;
		}
	}

	public class EventBinder<TEventHandler>
	{
		private readonly List<Type> eventTypes = new List<Type>(); 

		public EventBinder<TEventHandler> Handler<THandler>()
			where THandler : TEventHandler
		{
			eventTypes.Add(typeof(THandler));

			return this;
		}

		internal IEnumerable<Type> GetHandlerTypes()
		{
			return eventTypes;
		}
	}

	public class QueryBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type queryType;

		public QueryBuilder(BusBuilder busBuilder, Type queryType)
		{
			this.busBuilder = busBuilder;
			this.queryType = queryType;
		}

		public BusBuilder To(Type queryHandlerType, Pipeline pipeline)
		{
			busBuilder.registrations.Add(item: new MessageRegistration(queryType, queryHandlerType, pipeline));

			return busBuilder;
		}
	}

	public class QueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly BusBuilder busBuilder;

		public QueryBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public BusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			busBuilder.registrations.Add(item: new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));

			return busBuilder;
		}
	}
}
