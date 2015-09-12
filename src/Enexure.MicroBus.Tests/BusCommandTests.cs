using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests
{
	[TestFixture]
	public class CommandTests
	{
		class Command : ICommand { }

		[UsedImplicitly]
		class CommandHandler : ICommandHandler<Command>
		{
			public Task Handle(Command Command)
			{
				return Task.FromResult(0);
			}
		}

		[Test]
		public void NoHandlerShouldThrow()
		{
			var bus = BusBuilder.BuildBus(b => b);

			var func = (Func<Task>)(() => bus.Send(new Command()));

			func.ShouldThrowExactly<NoRegistrationForMessageException>().WithMessage("No registration for message of type Command was found");
		}

		[Test]
		public async Task TestCommand()
		{
			var bus = BusBuilder.BuildBus(b => b.RegisterCommand<Command>().To<CommandHandler>());

			await bus.Send(new Command());

		}

		[Test]
		public void TestMissingCommand()
		{
			var bus = BusBuilder.BuildBus(b => b);

			new Func<Task>(() => bus.Send(new Command()))
				.ShouldThrow<NoRegistrationForMessageException>();

		}
	}
}
