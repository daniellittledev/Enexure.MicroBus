using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
    public class InvalidMessageTypeException : Exception
    {
        public InvalidMessageTypeException(Type getType, Type type)
            : base(string.Format("Message was of type '{0}' but an instance of type '{1}' was expected", getType.Name, type.Name))
        {
        }
    }
}