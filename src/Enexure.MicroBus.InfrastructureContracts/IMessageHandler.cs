using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IMessageHandler<in TMessage, TResult>
	{
		Task<TResult> Handle(TMessage message);
	}
}
