using System;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Enexure.MicroBus.Tests")]

namespace Enexure.MicroBus
{
	public static class ReflectionExtensions
	{
		public static TypeInfo GetTypeInfo<T>()
		{
			return typeof(T).GetTypeInfo();
		}

		public static bool ImplementsGenericType<T>(Type genericType)
		{
			return ImplementsGenericType(typeof(T).GetTypeInfo(), genericType);
		}

		public static bool ImplementsGenericType(this Type type, Type genericType)
		{
			return ImplementsGenericType(type, genericType);
		}

		public static bool ImplementsGenericType(this TypeInfo type, Type genericType)
		{
			var genericTypeInfo = genericType.GetTypeInfo();
			if (!genericTypeInfo.IsGenericType)
			{
				throw new ArgumentException("Parameter is not a generic type", nameof(genericType));
			}

			return type.ImplementedInterfaces.Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == genericType);
		}

		internal static IEnumerable<GenericMatch> GetGenericMatches(this TypeInfo typeInfo, Type genericType)
		{
			return typeInfo.ImplementedInterfaces
				.Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == genericType)
				.Select(x => new GenericMatch(x.GenericTypeArguments.First(), typeInfo.AsType()));
		}

		public static IEnumerable<Type> ExpandType(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			return GetAllImplementedTypes(type).Concat(typeInfo.ImplementedInterfaces);
		}

		private static IEnumerable<Type> GetAllImplementedTypes(Type type)
		{
			yield return type;

			var typeInfo = type.GetTypeInfo();
			if (typeInfo.BaseType != null)
			{
				foreach (var baseType in GetAllImplementedTypes(typeInfo.BaseType))
				{
					yield return baseType;
				}
			}
		}

		public static object GetTaskResult(Task task)
		{
			var taskType = task.GetType();
			var typeInfo = taskType.GetTypeInfo();
			if (!typeInfo.IsGenericType)
			{
				throw new SomehowRecievedTaskWithoutResultException();
			}

			var resultProperty = typeInfo.GetDeclaredProperty("Result").GetMethod;
			return resultProperty.Invoke(task, new object[] { });
		}

		public static Task CallHandleOnHandler(object handler, object message)
		{
			var type = handler.GetType();
			var messageType = message.GetType();

			var handleMethods = type.GetRuntimeMethods().Where(m => m.Name == "Handle");
			var handleMethod = handleMethods.Single(x => {
				var parameterTypeIsCorrect = x.GetParameters().Single().ParameterType.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo());
				return parameterTypeIsCorrect
					&& x.IsPublic
					&& ((x.CallingConvention & CallingConventions.HasThis) != 0);
			});

			var objectTask = handleMethod.Invoke(handler, new object[] { message });

			if (objectTask == null)
			{
				throw new NullReferenceException(string.Format("Handler for message of type '{0}' returned null.{1}To Resolve you can try{1} 1) Return a task instead", messageType, Environment.NewLine));
			}

			return (Task)objectTask;
		}
	}
}
