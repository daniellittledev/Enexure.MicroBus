using System.Threading.Tasks;

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
