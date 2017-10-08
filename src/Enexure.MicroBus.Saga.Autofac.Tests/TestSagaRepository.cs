using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enexure.MicroBus.Sagas;

namespace Enexure.MicroBus.Saga.Autofac.Tests
{
    public class TestSagaRepository : ISagaRepository<TestSaga>
    {
        Dictionary<Guid, TestSaga> sagas = new Dictionary<Guid, TestSaga>();

        public Task CompleteAsync(TestSaga saga)
        {
            sagas.Remove(saga.Id);
            return Task.CompletedTask;
        }

        public Task CreateAsync(TestSaga saga)
        {
            sagas.Add(saga.Id, saga);
            return Task.CompletedTask;
        }

        public Task<TestSaga> FindAsync(IEvent message)
        {
            var correlatedMessage = message as IHaveCorrelationId;
            if (correlatedMessage != null) {
                return FindById(correlatedMessage.CorrelationId);
            }

            throw new Exception("message must inherit from the interface IHaveCorrelationId");
        }

        public Task<TestSaga> FindById(Guid id)
        {
            return Task.FromResult(sagas.ContainsKey(id) ? sagas[id] : null);
        }

        public TestSaga NewSaga()
        {
            return new TestSaga();
        }

        public Task UpdateAsync(TestSaga saga)
        {
            return Task.CompletedTask;
        }
    }
}
