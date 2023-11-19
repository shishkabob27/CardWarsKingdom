using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Prime31.Reflection
{
	public class ReflectionUtils
	{
		public static Attribute getAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !Attribute.IsDefined(info, type))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(info, type);
		}

		public static Attribute getAttribute(Type objectType, Type attributeType)
		{
			if (objectType == null || attributeType == null || !Attribute.IsDefined(objectType, attributeType))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(objectType, attributeType);
		}

		public static bool isTypeGenericeCollectionInterface(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}

		public static bool isTypeDictionary(Type type)
		{
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				return true;
			}
			if (!type.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IDictionary<, >);
		}

		public static bool isNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static object toNullableType(object obj, Type nullableType)
		{
			return (obj != null) ? Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture) : null;
		}
	}
}
