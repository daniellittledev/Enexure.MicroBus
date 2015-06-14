namespace Enexure.MicroBus
{
	public interface IQuery<in TQuery, out TResult> : IMessage
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
	}
}
