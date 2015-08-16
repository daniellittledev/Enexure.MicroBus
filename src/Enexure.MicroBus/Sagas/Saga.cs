using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISaga
	{
		Guid Id { get; set; }
	}

	public interface ISaga<TData> : ISaga
		where TData : class
	{
		TData Data { get; set; }
	}

	public interface ISagaFinder<TData, in TMessage>
		where TMessage : IMessage
		where TData : class
	{
		ISaga<TData> FindBy(TMessage message);
	}

	public interface ISagaStartedBy<in TMessage> : IHandleMessage<TMessage>
		where TMessage : IMessage
	{
	}

	public interface IHandleMessage<in TMessage>
		where TMessage : IMessage
	{
		Task<bool> Handle(TMessage message);
	}

	public class Saga<TData> : ISaga<TData>
		where TData : class
	{
		public Guid Id { get; set; }
		public TData Data { get; set; }
	}

	public interface ISagaRepository
	{
		Task<ISaga> GetSagaForAsync(IMessage message);
		Task UpdateAsync(ISaga saga);
		Task CompleteAsync(ISaga saga);
	}

	internal class SomeSaga : Saga<SagaData>,
		ISagaStartedBy<FirstEvent>,
		ISagaStartedBy<SecondEvent>
	{
		public Task<bool> Handle(FirstEvent message)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Handle(SecondEvent message)
		{
			throw new NotImplementedException();
		}
	}

	internal class SecondEvent : IEvent
	{
	}

	internal class FirstEvent : IEvent
	{
	}

	internal class SagaData
	{
	}

	internal class SagaUsage
	{
		public async Task Do()
		{
			IBusBuilder busBuilder = new BusBuilder();
			//busBuilder.
			//	RegisterSaga<SomeSaga>(pipeline);

			var bus = busBuilder.BuildBus();

			var message = new SecondEvent();

			var repo = (ISagaRepository)null;

			var sagaOfUnknownType = await repo.GetSagaForAsync(message);

			var isCompleted = await ((ISagaStartedBy<SecondEvent>)sagaOfUnknownType).Handle(message);

			if (!isCompleted) {
				await repo.UpdateAsync(sagaOfUnknownType);
			} else {
				await repo.CompleteAsync(sagaOfUnknownType);
			}

		}
	}


}
