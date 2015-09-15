using System;
using System.Diagnostics;
using System.Linq;

namespace Enexure.MicroBus.Sagas
{
	public static class BusBuilderExtensions
	{
		public static IHandlerRegister RegisterSaga<TSaga>(this IHandlerRegister handlerRegister)
			where TSaga : ISaga
		{
			var sagaType = typeof(TSaga);
			return HandlerRegister(handlerRegister, sagaType);
		}

		public static IHandlerRegister RegisterSaga(this IHandlerRegister handlerRegister, Type sagaType)
		{
			var sagaInterfaces = sagaType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISaga<>)).ToList();

			if (!sagaInterfaces.Any()) throw new ArgumentException("Type must implement ISaga<T>", nameof(sagaType));
			if (sagaInterfaces.Count > 1) throw new ArgumentException("A Saga can only implement ISaga<T> once", nameof(sagaType));

			return HandlerRegister(handlerRegister, sagaType);
		}

		private static IHandlerRegister HandlerRegister(IHandlerRegister handlerRegister, Type sagaType)
		{
			var eventTypes = sagaType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>)).Select(x => x.GetGenericArguments().First());

			foreach (var eventType in eventTypes)
			{
				handlerRegister.RegisterEvent(eventType).To(typeof(SagaRunnerEventHandler<,>).MakeGenericType(sagaType, eventType));
			}

			return handlerRegister;
		}

	}
}
