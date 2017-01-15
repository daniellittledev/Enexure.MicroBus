using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	using System.Threading;

	public interface IMessageHandler<in TMessage, TResult>
	{
		Task<TResult> Handle(TMessage message);
	}

	public interface ICancelableMessageHandler<in TMessage, TResult>
	{
		Task<TResult> Handle(TMessage message, CancellationToken cancellation);
	}
}
