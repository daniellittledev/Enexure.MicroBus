using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public static class MessagesHelper
	{
		public static IEnumerable<Type> ExpandType(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			var isEvent = typeof(IEvent).GetTypeInfo().IsAssignableFrom(typeInfo);
			var isCommand = typeof(ICommand).GetTypeInfo().IsAssignableFrom(typeInfo);
			var isQuery = typeof(IQuery).GetTypeInfo().IsAssignableFrom(typeInfo);

			var eventInterface = typeof(IEvent).GetTypeInfo();
			var commandInterface = typeof(ICommand).GetTypeInfo();
			var queryInterface = typeof(IQuery).GetTypeInfo();

			return from subTypeInfo in typeInfo.ImplementedInterfaces.Select(x => x.GetTypeInfo()).Concat(GetAllBaseTypes(type.GetTypeInfo()))
			       where isEvent && eventInterface.IsAssignableFrom(subTypeInfo)
			          || isCommand && commandInterface.IsAssignableFrom(subTypeInfo)
			          || isQuery && queryInterface.IsAssignableFrom(subTypeInfo)
			       select subTypeInfo.AsType();
		}

		private static IEnumerable<TypeInfo> GetAllBaseTypes(TypeInfo type)
		{
			if (type.BaseType != null)
			{
				foreach (var baseType in GetAllBaseTypes(type.BaseType.GetTypeInfo()))
				{
					yield return baseType;
				}
			}

			if (type.GetType() != typeof(object)) {
				yield return type;
			}
		}
	}
}
