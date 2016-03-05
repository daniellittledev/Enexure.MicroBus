using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public class BusBuilder
	{
		private readonly List<HandlerRegistration> registrations = new List<HandlerRegistration>();
		private readonly List<InterceptorRegistration> interceptors =  new List<InterceptorRegistration>();

		public List<HandlerRegistration> Registrations
		{
			get { return registrations; }
		}

		public List<InterceptorRegistration> Interceptors
		{
			get { return interceptors; }
		}

		public BusBuilder RegisterCommandHandler<TCommand, TCommandHandler>()
			where TCommand : ICommand
			where TCommandHandler : ICommandHandler<TCommand>
		{
			registrations.Add(HandlerRegistration.New<TCommand, CommandHandlerShim<TCommand, TCommandHandler>>(new[] { typeof(TCommandHandler) }));
			return this;
		}

		public BusBuilder RegisterEventHandler<TEvent, TEventHandler>()
			where TEvent : IEvent
			where TEventHandler : IEventHandler<TEvent>
		{
			registrations.Add(HandlerRegistration.New<TEvent, EventHandlerShim<TEvent, TEventHandler>>(new[] { typeof(TEventHandler) }));
			return this;
		}

		public BusBuilder RegisterQueryHandler<TQuery, TResult, TQueryHandler>()
			where TQuery : IQuery<TQuery, TResult>
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			registrations.Add(HandlerRegistration.New<TQuery, QueryHandlerShim<TQuery, TResult, TQueryHandler>>(new[] { typeof(TQueryHandler) }));
			return this;
		}

		public BusBuilder RegisterMessage(HandlerRegistration registration)
		{
			registrations.Add(registration);
			return this;
		}

		public BusBuilder RegisterHandler<TMessage, TMessageHandler>()
			where TMessageHandler : IMessageHandler<TMessage, Unit>
		{
			registrations.Add(HandlerRegistration.New<TMessage, TMessageHandler>());
			return this;
		}

		public BusBuilder RegisterHandler<TMessage, TResult, TMessageHandler>()
			where TMessageHandler : IMessageHandler<TMessage, TResult>
		{
			registrations.Add(HandlerRegistration.New<TMessage, TMessageHandler>());
			return this;
		}

		public BusBuilder RegisterHandlers(Assembly assembly)
		{
			return RegisterHandlers(new[] { assembly }, x => true);
		}

		public BusBuilder RegisterHandlers(Assembly assembly, Func<Type, bool> predicate)
		{
			return RegisterHandlers(new[] { assembly }, predicate);
		}

		public BusBuilder RegisterHandlers(IEnumerable<Assembly> assemblies)
		{
			return RegisterHandlers(assemblies, x => true);
		}

		public BusBuilder RegisterHandlers(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
		{
			var handlerRegistrations = assemblies
				.SelectMany(AllTheTypes)
				.Where(x => predicate(x.AsType()))
				.SelectMany(HandlersOnTheTypes)
				.Select(HandlersAsRegistrations);

			registrations.AddRange(handlerRegistrations);
			return this;
		}

		[Obsolete("Use RegisterInterceptor instead")]
		public BusBuilder RegisterPipelineHandler<T>()
		{
			interceptors.Add(new InterceptorRegistration(typeof(PipelineHandlerToInterceptorConverter<>).MakeGenericType(typeof(T)), new[] { typeof(T) }));
			return this;
		}

		private HandlerRegistration HandlersAsRegistrations(GenericMatch match)
		{
			return new HandlerRegistration(match.MessageType, match.HandlerType);
		}

		private IEnumerable<GenericMatch> HandlersOnTheTypes(TypeInfo type)
		{
			return ReflectionExtensions.GetGenericMatches(type, typeof(ICommandHandler<>))
				.Concat(ReflectionExtensions.GetGenericMatches(type, typeof(IEventHandler<>)))
				.Concat(ReflectionExtensions.GetGenericMatches(type, typeof(IQueryHandler<,>)))
				.Concat(ReflectionExtensions.GetGenericMatches(type, typeof(IMessageHandler<,>)))
				;
		}

		private IEnumerable<TypeInfo> AllTheTypes(Assembly assembly)
		{
			return assembly.DefinedTypes;
		}

		public BusBuilder RegisterInterceptor<TInterceptor>()
			where TInterceptor : IInterceptor
		{
			interceptors.Add(new InterceptorRegistration(typeof(TInterceptor)));
			return this;
		}
	}

	internal class PipelineHandlerToInterceptorConverter<T> : IInterceptor
		where T : IPipelineHandler
	{
		private readonly IPipelineHandler handler;

		public PipelineHandlerToInterceptorConverter(T handler)
		{
			this.handler = handler;
		}

		public async Task<IReadOnlyCollection<object>> Handle(IInterceptorChain next, object message)
		{
			return (IReadOnlyCollection<object>)await handler.Handle(async x => await next.Handle(x), message as IMessage);
		}
	}

	internal class CommandHandlerShim<TCommand, TCommandHandler> : IMessageHandler<TCommand, Unit>
		where TCommandHandler : ICommandHandler<TCommand>
		where TCommand : ICommand
	{
		private readonly TCommandHandler handler;

		public CommandHandlerShim(TCommandHandler handler)
		{
			this.handler = handler;
		}

		public async Task<Unit> Handle(TCommand message)
		{
			await handler.Handle(message);
			return Unit.Unit;
		}
	}

	internal class EventHandlerShim<TEvent, TEventHandler> : IMessageHandler<TEvent, Unit>
		where TEventHandler : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly TEventHandler handler;

		public EventHandlerShim(TEventHandler handler)
		{
			this.handler = handler;
		}

		public async Task<Unit> Handle(TEvent message)
		{
			await handler.Handle(message);
			return Unit.Unit;
		}
	}

	internal class QueryHandlerShim<TQuery, TResult, TQueryHandler> : IMessageHandler<TQuery, TResult>
		where TQueryHandler : IQueryHandler<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
	{
		private readonly TQueryHandler handler;

		public QueryHandlerShim(TQueryHandler handler)
		{
			this.handler = handler;
		}

		public async Task<TResult> Handle(TQuery query)
		{
			return await handler.Handle(query);
		}
	}
}
