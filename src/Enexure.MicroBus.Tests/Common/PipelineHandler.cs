using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus.Tests
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
