namespace Enexure.MicroBus
{
	public interface IQuery<in TQuery, out TResult> : IQuery, IMessage
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
	}
	public interface IQuery
	{
	}
}