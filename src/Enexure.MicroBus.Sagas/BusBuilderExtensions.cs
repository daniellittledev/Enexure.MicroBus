using System;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus.Sagas
{
    public static class BusBuilderExtensions
    {
        public static BusBuilder RegisterSaga<TSaga>(this BusBuilder busBuilder)
            where TSaga : ISaga
        {
            return busBuilder.RegisterSaga(typeof(TSaga), FinderList.Empty);
        }

        public static BusBuilder RegisterSaga<TSaga>(this BusBuilder busBuilder, FinderList sagaFinders)
            where TSaga : ISaga
        {
            return busBuilder.RegisterSaga(typeof(TSaga), sagaFinders);
        }

        public static BusBuilder RegisterSaga(this BusBuilder busBuilder, Type sagaType)
        {
            return busBuilder.RegisterSaga(sagaType, FinderList.Empty);
        }

        public static BusBuilder RegisterSaga(this BusBuilder busBuilder, Type sagaType, FinderList sagaFinders)
        {
            var sagaInterfaces = sagaType.GetTypeInfo().ImplementedInterfaces
                .Where(x => x == typeof(ISaga))
                .ToList();

            if (!sagaInterfaces.Any()) throw new ArgumentException("Type must implement ISaga", nameof(sagaType));
            if (sagaInterfaces.Count > 1) throw new ArgumentException("A Saga can only implement ISaga once", nameof(sagaType));

            var eventTypes = sagaType.GetTypeInfo().ImplementedInterfaces
                .Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(x => x.GenericTypeArguments.First());

            foreach (var eventType in eventTypes)
            {
                busBuilder = busBuilder.RegisterMessage(
                    new HandlerRegistration(eventType, typeof(SagaRunnerEventHandler<,>).MakeGenericType(sagaType, eventType),
                        new Type[] { sagaType }.Concat(sagaFinders).ToArray()
                    )
                );
            }

            return busBuilder;
        }
    }
}
