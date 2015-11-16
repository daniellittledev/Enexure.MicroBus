using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IPipelineBuilder
	{
		Func<IMessage, Task<object>> GetPipelineForMessage(Type messageType);
	}
}