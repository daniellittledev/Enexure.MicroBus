using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class HandlerProvider : IHandlerProvider
	{
		private readonly IDictionary<Type, GroupedMessageRegistration> registrationsLookup;

		public HandlerProvider(IEnumerable<MessageRegistration> registrations)
		{
			var tempRegistrationsLookup = new Dictionary<Type, GroupedMessageRegistration>();

			var groups = registrations.GroupBy(x => new {x.MessageType, x.Pipeline});

			foreach (var group in groups) {

				var pipelineGroups = @group
					.GroupBy(x => x.Pipeline)
					.ToList();

				if (pipelineGroups.Count > 1) {
					throw new MultipleDifferentPipelinesRegisteredException(group.Key.MessageType, pipelineGroups.Select(x=> x.Key).ToList());
				}

				var pipelineRegistrations = pipelineGroups
					.Select(pipelineGroup => new GroupedMessageRegistration(pipelineGroup.Key, pipelineGroup.Select(x => x.Handler).ToArray()))
					.ToList();

				tempRegistrationsLookup.Add(group.Key.MessageType, pipelineRegistrations.Single());
			}

			registrationsLookup = new Dictionary<Type, GroupedMessageRegistration>(tempRegistrationsLookup);

			foreach (var registration in tempRegistrationsLookup) {

				var handlers = GetAllHandlers(tempRegistrationsLookup, registration.Key, registration.Value.Pipeline).ToList();

				registrationsLookup[registration.Key] = new GroupedMessageRegistration(registration.Value.Pipeline, handlers);
			}
		}

		private IEnumerable<Type> GetAllHandlers(Dictionary<Type, GroupedMessageRegistration> tempRegistrationsLookup, Type messageType, Pipeline pipeline)
		{
			var types = GetAllBaseTypesAndInheritedInterfaces(messageType).Where(tempRegistrationsLookup.ContainsKey);
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

		private IEnumerable<Type> GetAllBaseTypesAndInheritedInterfaces(Type type)
		{
			foreach (var @interface in GetAllInterfaces(type)) {
				yield return @interface;
			}
			foreach (var baseType in GetAllBaseTypes(type)) {
				yield return baseType;
			}
		}

		private IEnumerable<Type> GetAllInterfaces(Type type)
		{
			foreach (var @interface in type.GetInterfaces()) {
				foreach (var result in GetAllInterfaces(@interface)) {
					yield return result;
				}
			}
			if (type.IsInterface) {
				yield return type;
			}
		}

		private IEnumerable<Type> GetAllBaseTypes(Type type)
		{
			if (type.BaseType != null) {
				foreach (var baseType in GetAllBaseTypes(type.BaseType)) {
					yield return baseType;
				}
			}
			yield return type;
		}

		public bool GetRegistrationForMessage(Type commandType, out GroupedMessageRegistration registration)
		{
			return registrationsLookup.TryGetValue(commandType, out registration);
		}

	}
}