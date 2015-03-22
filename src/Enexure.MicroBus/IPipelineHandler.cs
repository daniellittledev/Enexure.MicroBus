using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IPipelineHandler
	{
		Task Handle(IMessage message);
	}
}
