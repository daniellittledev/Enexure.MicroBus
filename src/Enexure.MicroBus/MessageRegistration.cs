using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class HandlerRegistration
	{
		public Type MessageType { get; }
		public Type HandlerType { get; }
		public IEnumerable<Type> Dependencies { get; }

		public HandlerRegistration(Type messageType, Type handlerType)
		{
			this.MessageType = messageType;
			this.HandlerType = handlerType;
			this.Dependencies = new Type[] { };

			Validate();
		}

		public HandlerRegistration(Type messageType, Type handlerType, IEnumerable<Type> dependencies)
		{
			this.MessageType = messageType;
			this.HandlerType = handlerType;
			this.Dependencies = dependencies;

			Validate();
		}

		private void Validate()
		{
			var handlerMessageTypes = HandlerType.GetTypeInfo()
				.ImplementedInterfaces
				.Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IMessageHandler<,>))
				.Select(x => x.GenericTypeArguments.First())
				.ToArray();

			if (handlerMessageTypes.Any() && handlerMessageTypes.All(x => !x.GetTypeInfo().IsAssignableFrom(MessageType.GetTypeInfo())))
			{
				throw new ArgumentException(string.Format("The handler {0} cannot handle message of type {1}", HandlerType.Name, MessageType.Name));
			}

		}

		public static HandlerRegistration New<TMessage, THandler>()
		{
			return new HandlerRegistration(typeof(TMessage), typeof(THandler), new Type[] { });
		}

		public static HandlerRegistration New<TMessage, THandler>(IEnumerable<Type> dependencies)
		{
			return new HandlerRegistration(typeof(TMessage), typeof(THandler), dependencies);
		}

	}
}
