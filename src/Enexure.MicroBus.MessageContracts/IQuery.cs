using System;

namespace Enexure.MicroBus.MessageContracts
{
	public interface IQuery<TQuery, TResult> : IMessage
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
	}
}
