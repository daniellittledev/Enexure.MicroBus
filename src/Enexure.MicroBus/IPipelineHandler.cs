using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IPipelineHandler
	{
		Task<object> Handle(IMessage message);
	}
}
