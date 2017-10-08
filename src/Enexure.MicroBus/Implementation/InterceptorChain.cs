using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    internal class NextHandlerRunner : INextHandler
    {
        private Func<object, Task<object>> handle;

        public NextHandlerRunner(Func<object, Task<object>> handle)
        {
            this.handle = handle;
        }

        public Task<object> Handle(object message)
        {
            return handle(message);
        }
    }
}