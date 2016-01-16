using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class HandlerRegister : IHandlerRegister
	{
		private readonly ImmutableList<MessageRegistration> registrations;

		public HandlerRegister()
		{
			registrations = ImmutableList<MessageRegistration>.Empty;
		}

		public HandlerRegister(IEnumerable<MessageRegistration> registrations)
		{
			this.registrations = registrations as ImmutableList<MessageRegistration> ?? ImmutableList<MessageRegistration>.Empty.AddRange(registrations);
		}

		public HandlerRegister(HandlerRegister handlerRegister, MessageRegistration registration)
		{
			this.registrations = handlerRegister.registrations.Add(registration);
		}

		public HandlerRegister(HandlerRegister handlerRegister, IEnumerable<MessageRegistration> registrations)
		{
			this.registrations = handlerRegister.registrations.AddRange(registrations);
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

		public IHandlerRegister RegisterMessage(MessageRegistration messageRegistration)
		{
			return new HandlerRegister(this, messageRegistration);
		}

		public IHandlerRegister RegisterTypes(params Assembly[] assemblies)
		{
			return RegisterTypes(null, Pipeline.EmptyPipeline, assemblies);
		}

		public IHandlerRegister RegisterTypes(Func<TypeInfo, bool> predicate, params Assembly[] assemblies)
		{
			return RegisterTypes(predicate, Pipeline.EmptyPipeline, assemblies);
		}

		public IHandlerRegister RegisterTypes(Pipeline pipeline, params Assembly[] assemblies)
		{
			return RegisterTypes(null, pipeline, assemblies);
		}

		public IHandlerRegister RegisterTypes(Func<TypeInfo, bool> predicate, Pipeline pipeline, params Assembly[] assemblies)
		{
			var noPredicate = (predicate == null);

			var types = new [] {
				typeof(ICommandHandler<>),
				typeof(IEventHandler<>),
				typeof(IQueryHandler<,>)
			};

			var matchingTypes = assemblies
				.SelectMany(x => x.DefinedTypes)
				.Where(x => x.IsClass && !x.IsGenericType)
				.SelectMany(x => x.ImplementedInterfaces.Select(y => new { Type = x, Interface = y }))
				.Where(x => x.Interface.GetTypeInfo().IsGenericType 
					&& types.Contains(x.Interface.GetGenericTypeDefinition()) 
					&& (noPredicate || predicate(x.Type)));

			var messageRegistrations = new List<MessageRegistration>();

			foreach (var item in matchingTypes) {

				var messageType = item.Interface.GetTypeInfo().GenericTypeArguments.First();

				if (!item.Type.ImplementedInterfaces.Any(i => i.Name == "ISaga")) {
					messageRegistrations.Add(new MessageRegistration(messageType, item.Type.GetType(), pipeline));
				}
			}

			return new HandlerRegister(this, messageRegistrations); ;
		}

	}
}
