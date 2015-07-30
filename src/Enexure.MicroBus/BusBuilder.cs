using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class BusBuilder : IBusBuilder
	{
		private readonly ImmutableList<MessageRegistration> registrations;

		public BusBuilder()
		{
			registrations = ImmutableList<MessageRegistration>.Empty;
		}

		public BusBuilder(IEnumerable<MessageRegistration> registrations)
		{
			this.registrations = registrations as ImmutableList<MessageRegistration> ?? ImmutableList<MessageRegistration>.Empty.AddRange(registrations);
		}

		public BusBuilder(BusBuilder busBuilder, MessageRegistration registrations)
		{
			this.registrations = busBuilder.registrations.Add(registrations);
		}

		public ICommandBuilder<TCommand> RegisterCommand<TCommand>()
			where TCommand : ICommand
		{
			return new CommandBuilder<TCommand>(this);
		}

		public ICommandBuilder RegisterCommand(Type type)
		{
			return new CommandBuilder(this, type);
		}

		public IEventBuilder<TEvent> RegisterEvent<TEvent>()
			where TEvent : IEvent
		{
			return new EventBuilder<TEvent>(this);
		}

		public IEventBuilder RegisterEvent(Type type)
		{
			return new EventBuilder(this, type);
		}

		public IQueryBuilder<TQuery, TResult> RegisterQuery<TQuery, TResult>()
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult
		{
			return new QueryBuilder<TQuery, TResult>(this);
		}

		public IQueryBuilder RegisterQuery(Type type)
		{
			return new QueryBuilder(this, type);
		}

		public IReadOnlyCollection<MessageRegistration> GetMessageRegistrations()
		{
			return registrations;
		}

		public IHandlerRegistar BuildHandlerRegistar()
		{
			return new HandlerRegistar(registrations);
		}

	    public IMicroBus BuildBus()
	    {
	        return BuildBus(new BusSettings());
	    }

	    public IMicroBus BuildBus(BusSettings busSettings)
		{
			return new MicroBus(new HandlerBuilder(BuildHandlerRegistar(), busSettings), new DefaultDependencyResolver());
		}
	}

	public interface ICommandBuilder
	{
		IBusBuilder To(Type commandHandlerType, Pipeline pipeline);
	}

	public class CommandBuilder : ICommandBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type commandType;

		public CommandBuilder(BusBuilder busBuilder, Type commandType)
		{
			this.busBuilder = busBuilder;
			this.commandType = commandType;
		}

		public IBusBuilder To(Type commandHandlerType, Pipeline pipeline)
		{
			return new BusBuilder(busBuilder, new MessageRegistration(commandType, commandHandlerType, pipeline));
		}
	}

	public interface ICommandBuilder<out TCommand>
		where TCommand : ICommand
	{
		IBusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>;
	}

	public class CommandBuilder<TCommand> : ICommandBuilder<TCommand>
		where TCommand : ICommand
	{
		private readonly BusBuilder busBuilder;

		public CommandBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TCommandHandler>(Pipeline pipeline)
			where TCommandHandler : ICommandHandler<TCommand>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TCommand), typeof(TCommandHandler), pipeline)); 
		}
	}

	public interface IEventBuilder
	{
		IBusBuilder To(Type eventHandlerType, Pipeline pipeline);
		IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline);
	}

	public class EventBuilder : IEventBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type eventType;

		public EventBuilder(BusBuilder busBuilder, Type eventType)
		{
			this.busBuilder = busBuilder;
			this.eventType = eventType;
		}

		public IBusBuilder To(Type eventHandlerType, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(eventType, eventHandlerType, pipeline));
		}

		public IBusBuilder To(IEnumerable<Type> eventHandlerTypes, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(eventType, eventHandlerTypes, pipeline));
		}
	}

	public interface IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		IBusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>;

		IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline);
	}

	public class EventBuilder<TEvent> : IEventBuilder<TEvent>
		where TEvent : IEvent
	{
		private readonly BusBuilder busBuilder;

		public EventBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TEventHandler>(Pipeline pipeline)
			where TEventHandler : IEventHandler<TEvent>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TEvent), typeof(TEventHandler), pipeline));
		}

		public IBusBuilder To(Action<IEventBinder<TEvent>> eventBinder, Pipeline pipeline)
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			var binder = new EventBinder<TEvent>();
			eventBinder(binder);

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TEvent), binder.GetHandlerTypes(), pipeline));
		}
	}

	public interface IEventBinder<TEvent> where TEvent : IEvent
	{
		IEventBinder<TEvent> Handler<THandler>()
			where THandler : IEventHandler<TEvent>;
	}

	public class EventBinder<TEvent> : IEventBinder<TEvent>
		where TEvent : IEvent
	{
		private readonly List<Type> eventTypes = new List<Type>();

		public IEventBinder<TEvent> Handler<THandler>()
			where THandler : IEventHandler<TEvent>
		{
			eventTypes.Add(typeof(THandler));

			return this;
		}

		internal IEnumerable<Type> GetHandlerTypes()
		{
			return eventTypes;
		}
	}

	public interface IQueryBuilder
	{
		IBusBuilder To(Type queryHandlerType, Pipeline pipeline);
	}

	public class QueryBuilder : IQueryBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type queryType;

		public QueryBuilder(BusBuilder busBuilder, Type queryType)
		{
			this.busBuilder = busBuilder;
			this.queryType = queryType;
		}

		public IBusBuilder To(Type queryHandlerType, Pipeline pipeline)
		{
			return new BusBuilder(busBuilder, new MessageRegistration(queryType, queryHandlerType, pipeline));
		}
	}

	public interface IQueryBuilder<out TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		IBusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>;
	}

	public class QueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly BusBuilder busBuilder;

		public QueryBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));
		}
	}
}
