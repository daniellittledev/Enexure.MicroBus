using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class HandlerProvider : IHandlerProvider
	{
		private readonly IDictionary<Type, GroupedMessageRegistration> registrationsLookup;

		private HandlerProvider(IDictionary<Type, GroupedMessageRegistration> registrations)
		{
			registrationsLookup = registrations;
		}

		public static HandlerProvider Create(IEnumerable<MessageRegistration> registrations)
		{
			return new HandlerProvider(GroupRegistrations(registrations));
		}

		private static Dictionary<Type, GroupedMessageRegistration> GroupRegistrations(IEnumerable<MessageRegistration> registrations)
		{
			var tempRegistrationsLookup = new Dictionary<Type, GroupedMessageRegistration>();

			var groups = registrations.GroupBy(x => new {x.MessageType, x.Pipeline});

			foreach (var group in groups) {

				var messageType = group.Key.MessageType;
				if (!typeof(IEvent).IsAssignableFrom(messageType) && group.Skip(1).Any()) {

					if (typeof(ICommand).IsAssignableFrom(messageType)) {

						throw new MultipleRegistrationsWithTheSameCommandException(messageType);
					} else {

						throw new MultipleRegistrationsWithTheSameQueryException(messageType);
					}
				}

				var pipelineGroups = group
					.GroupBy(x => x.Pipeline)
					.ToList();

				if (pipelineGroups.Count > 1) {
					throw new MultipleDifferentPipelinesRegisteredException(messageType, pipelineGroups.Select(x => x.Key).ToList());
				}

				var pipelineRegistrations = pipelineGroups
					.Select(pipelineGroup => new GroupedMessageRegistration(pipelineGroup.Key, pipelineGroup.Select(x => x.Handler).ToArray()))
					.ToList();

				tempRegistrationsLookup.Add(messageType, pipelineRegistrations.Single());
			}

			var registrationsLookup = new Dictionary<Type, GroupedMessageRegistration>(tempRegistrationsLookup);

			foreach (var registration in tempRegistrationsLookup) {
				var handlers = GetAllHandlers(tempRegistrationsLookup, registration.Key, registration.Value.Pipeline).ToList();

				registrationsLookup[registration.Key] = new GroupedMessageRegistration(registration.Value.Pipeline, handlers);
			}

			return registrationsLookup;
		}

		private static IEnumerable<Type> GetAllHandlers(Dictionary<Type, GroupedMessageRegistration> tempRegistrationsLookup, Type messageType, Pipeline pipeline)
		{
			var types = Messages.ExpandType(messageType).Where(tempRegistrationsLookup.ContainsKey);
			foreach (var type in types) {
				var registration = tempRegistrationsLookup[type];

				if (registration.Pipeline != pipeline) {
					throw new MultipleDifferentPipelinesRegisteredException(messageType);
				}

				foreach (var handler in registration.Handlers) {
					yield return handler;
				}
			}
		}

		public bool GetRegistrationForMessage(Type commandType, out GroupedMessageRegistration registration)
		{
			return registrationsLookup.TryGetValue(commandType, out registration);
		}

	}
}