using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IInterceptor
	{
		Task<IReadOnlyCollection<object>> Handle(IInterceptorChain next, object message);
	}

	public interface IInterceptorChain
	{
		Task<IReadOnlyCollection<object>> Handle(object message);
	}
}