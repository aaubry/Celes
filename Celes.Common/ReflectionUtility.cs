using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Celes.Common
{
	internal static class ReflectionUtility
	{
		private static MethodInfo GetGenericMethodFromExpression(LambdaExpression method)
		{
			var methodCall = method.Body as MethodCallExpression;
			if (methodCall == null)
			{
				throw new ArgumentException("The expression must be a method call.", "method");
			}
			return methodCall.Method.GetGenericMethodDefinition();
		}

		public static MethodInfo GetGenericMethod(Expression<Action> methodCall)
		{
			return GetGenericMethodFromExpression(methodCall);
		}

		public static MethodInfo GetGenericMethod<T>(Expression<Action<T>> methodCall)
		{
			return GetGenericMethodFromExpression(methodCall);
		}

		public static Type GetImplementationOf(this Type target, Type genericType)
		{
			if (!genericType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("The type must be a generic type definition.", "genericType");
			}

			return GetImplementationRecursive(target, genericType);
		}

		private static Type GetImplementationRecursive(Type target, Type genericType)
		{
			if (target == null || target == typeof(object))
			{
				return null;
			}

			if (target.IsGenericType && target.GetGenericTypeDefinition() == genericType)
			{
				return target;
			}

			if (genericType.IsClass)
			{
				return GetImplementationRecursive(target.BaseType, genericType);
			}
			else
			{
				foreach (var itf in target.GetInterfaces())
				{
					var result = GetImplementationRecursive(itf, genericType);
					if (result != null)
					{
						return result;
					}
				}

				return null;
			}
		}
	}
}