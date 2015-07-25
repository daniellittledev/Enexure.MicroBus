using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests.Common
{
	class PipelineHandler : IPipelineHandler
	{
		public Task<object> Handle(Func<IMessage, Task<object>> next, IMessage message)
		{
			return next(message);
		}
	}
}
