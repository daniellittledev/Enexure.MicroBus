using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
    public class Pipeline
    {
        public IReadOnlyCollection<Type> DelegatingHandlerTypes { get; }
        public IReadOnlyCollection<Type> HandlerTypes { get; }

        public Pipeline(IReadOnlyCollection<Type> delegatingHandlerTypes, IReadOnlyCollection<Type> handlerTypes)
        {
            DelegatingHandlerTypes = delegatingHandlerTypes;
            HandlerTypes = handlerTypes;
        }
    }
}