using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IHandlerBuilder
	{
	    Func<IMessage, Task<object>> GetRunnerForMessage(IDependencyScope scope, Type messageType);
	}
}