using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus.Sagas
{
	public static class BusBuilderExtensions
	{
		public static IHandlerRegister RegisterSaga<TSaga>(this IHandlerRegister handlerRegister)
			where TSaga : ISaga
		{
			return handlerRegister.RegisterSaga<TSaga>(Pipeline.EmptyPipeline, SagaFinders.Empty);
		}

		public static IHandlerRegister RegisterSaga<TSaga>(this IHandlerRegister handlerRegister, SagaFinders sagaFinders)
			where TSaga : ISaga
		{
			return handlerRegister.RegisterSaga<TSaga>(Pipeline.EmptyPipeline, sagaFinders);
		}

		public static IHandlerRegister RegisterSaga<TSaga>(this IHandlerRegister handlerRegister, Pipeline pipeline, SagaFinders sagaFinders)
			where TSaga : ISaga
		{
			var sagaType = typeof(TSaga);
			return HandlerRegister(handlerRegister, sagaType, pipeline, sagaFinders);
		}

		public static IHandlerRegister RegisterSaga(this IHandlerRegister handlerRegister, Type sagaType, Pipeline pipeline)
		{
			return HandlerRegister(handlerRegister, sagaType, pipeline, SagaFinders.Empty);
		}

		public static IHandlerRegister RegisterSaga(this IHandlerRegister handlerRegister, Type sagaType, SagaFinders sagaFinders)
		{
			return handlerRegister.RegisterSaga(sagaType, Pipeline.EmptyPipeline, sagaFinders);
		}

		public static IHandlerRegister RegisterSaga(this IHandlerRegister handlerRegister, Type sagaType, Pipeline pipeline, SagaFinders sagaFinders)
		{
			var sagaInterfaces = sagaType.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISaga<>)).ToList();

			if (!sagaInterfaces.Any()) throw new ArgumentException("Type must implement ISaga<T>", nameof(sagaType));
			if (sagaInterfaces.Count > 1) throw new ArgumentException("A Saga can only implement ISaga<T> once", nameof(sagaType));

			return HandlerRegister(handlerRegister, sagaType, pipeline, sagaFinders);
		}

		private static IHandlerRegister HandlerRegister(IHandlerRegister handlerRegister, Type sagaType, Pipeline pipeline, SagaFinders sagaFinders)
		{
			var eventTypes = sagaType.GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
				.Select(x => x.GetGenericArguments().First());

			foreach (var eventType in eventTypes)
			{
				handlerRegister = handlerRegister.RegisterMessage(
					new MessageRegistration(eventType, typeof(SagaRunnerEventHandler<,>).MakeGenericType(sagaType, eventType), pipeline,
						new Type[] { sagaType },
						sagaFinders.ToList()
					)
				);
			}

			return handlerRegister;
		}

	}

	public class SagaFinders : IEnumerable<Type>
	{
		List<Type> sagaFinders = new List<Type>();

		public static SagaFinders Empty { get {
				return new SagaFinders();
			} }

		public void AddSagaFinder<TSagaFinder>()
		{
			var isActuallyASagaFinder = typeof(TSagaFinder)
				.GetInterfaces()
				.Where(i => i.IsGenericType)
				.Select(i => i.GetGenericTypeDefinition())
				.Contains(typeof(ISagaFinder<,>));

			if (!isActuallyASagaFinder) {
				throw new ArgumentException($"The Saga finder you passed in must implement the interface TSagaFinder", nameof(TSagaFinder));
			}

			sagaFinders.Add(typeof(TSagaFinder));
		}

		public IEnumerator<Type> GetEnumerator()
		{
			return sagaFinders.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return sagaFinders.GetEnumerator();
		}
	}
}
