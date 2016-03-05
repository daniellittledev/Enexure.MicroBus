using System;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public static class MessageTypeExtensions
	{
		public static MessageType GetMessageType(this Type messageType)
		{
			var typeInfo = messageType.GetTypeInfo();

			if (typeof(ICommand).GetTypeInfo().IsAssignableFrom(typeInfo)) {
				return MessageType.Command;

			} else if (typeof(IEvent).GetTypeInfo().IsAssignableFrom(typeInfo)) {
				return MessageType.Event;

			} else if (typeInfo.ImplementedInterfaces
				.Where(i => i.GetTypeInfo().IsGenericType)
				.Any(i => i.GetGenericTypeDefinition() == typeof(IQuery<,>))) {
				return MessageType.QueryAsync;

			} else {
				throw new NotSupportedException(string.Format("The message type {0} is not supported", messageType));
			}
		}
	}
}