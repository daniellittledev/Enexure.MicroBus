using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus.Tests.Common
{
	class PipelineHandler : IPipelineHandler
	{
		private readonly IPipelineHandler innerHandler;

		public PipelineHandler(IPipelineHandler innerHandler)
		{
			this.innerHandler = innerHandler;
		}

		public Task<object> Handle(IMessage message)
		{
			return innerHandler.Handle(message);
		}
	}
}
