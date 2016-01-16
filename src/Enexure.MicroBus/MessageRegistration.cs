using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class MessageRegistration
	{
		public MessageRegistration(Type messageType, Type handlerType)
			: this(messageType, handlerType, Pipeline.EmptyPipeline)
		{
		}

		public MessageRegistration(Type messageType, Type handlerType, Pipeline pipeline)
			: this(messageType, handlerType, pipeline, new Type[] { }, new Type[] { })
		{
		}

		public MessageRegistration(Type messageType, Type handlerType, Pipeline pipeline, IReadOnlyCollection<Type> dependancies, IReadOnlyCollection<Type> scopedDependancies)
		{
			if (messageType == null) throw new ArgumentNullException(nameof(messageType));
			if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
			if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));
			if (dependancies == null) throw new ArgumentNullException(nameof(dependancies));
			if (scopedDependancies == null) throw new ArgumentNullException(nameof(scopedDependancies));

			var messageTypeInfo = messageType.GetTypeInfo();

			if (!typeof(IMessage).GetTypeInfo().IsAssignableFrom(messageTypeInfo)) throw new ArgumentException($"Parameter {nameof(messageType)} must implement IMessage", nameof(messageType));

			if (typeof(ICommand).GetTypeInfo().IsAssignableFrom(messageTypeInfo)) {
				GetValue(new[] { messageType }, messageType, handlerType, type => typeof(ICommandHandler<>).MakeGenericType(type));

			} else if (typeof(IEvent).GetTypeInfo().IsAssignableFrom(messageTypeInfo)) {
				GetValue(MessagesHelper.ExpandType(messageType).Where(x => x != typeof(IMessage)), messageType, handlerType, type => typeof(IEventHandler<>).MakeGenericType(type));

			} else if (typeof(IQuery).GetTypeInfo().IsAssignableFrom(messageTypeInfo)) {

				var genericArguments = messageTypeInfo.ImplementedInterfaces.Single(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuery<,>)).GenericTypeArguments;
				GetValue(new[] { messageType }, messageType, handlerType, type => typeof(IQueryHandler<,>).MakeGenericType(genericArguments));
			}

			this.MessageType = messageType;
			this.Pipeline = pipeline;
			this.Handler = handlerType;

			Dependancies = dependancies;
			ScopedDependancies = scopedDependancies;
		}

		private static void GetValue(IEnumerable<Type> possibleMessageTypes, Type messageType, Type handlerType, Func<Type, Type> messageToHandler)
		{
			var handlerTypeInfo = handlerType.GetTypeInfo();
			var anyMatchingHandlers = possibleMessageTypes
				.Select(messageToHandler)
				.Select(x => x.GetTypeInfo())
				.Any(possibleTypeMatch => possibleTypeMatch.IsAssignableFrom(handlerTypeInfo));

			if (!anyMatchingHandlers) throw new ArgumentException($"The handler {handlerType.FullName} cannot handle the message type {messageType.FullName}. Check that the handler implements the correct interface.", nameof(handlerType));
		}

		public Type MessageType { get; }

		public Type Handler { get; }

		public Pipeline Pipeline { get; }

		public IReadOnlyCollection<Type> Dependancies { get; }

		public IReadOnlyCollection<Type> ScopedDependancies { get; }
	}
}