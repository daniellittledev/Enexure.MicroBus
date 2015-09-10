using System;
using System.Linq;

namespace Enexure.MicroBus
{
    public static class MessageTypeExtensions
    {
        public static MessageType GetMessageType(this Type messageType)
        {
            if (typeof(ICommand).IsAssignableFrom(messageType)) {
                return MessageType.Command;

            } else if (typeof(IEvent).IsAssignableFrom(messageType)) {
                return MessageType.Event;

            } else if (messageType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IQuery<,>))) {
                return MessageType.Query;

            } else {
                throw new NotSupportedException(string.Format("The message type {0} is not supported", messageType));
            }
        }
    }
}