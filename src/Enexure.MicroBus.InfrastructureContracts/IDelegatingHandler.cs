using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IDelegatingHandler
	{
		Task<object> Handle(INextHandler next, object message);
	}

	public interface INextHandler
	{
		Task<object> Handle(object message);
	}
}