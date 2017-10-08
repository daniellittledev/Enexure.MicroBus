using System;

namespace Enexure.MicroBus
{
    internal class GenericMatch
    {
        public Type MessageType { get; }
        public Type HandlerType { get;  }

        public GenericMatch(Type messageType, Type handlerType)
        {
            this.MessageType = messageType;
            this.HandlerType = handlerType;
        }
    }
}