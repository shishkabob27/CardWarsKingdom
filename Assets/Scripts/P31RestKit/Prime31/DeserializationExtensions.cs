using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Prime31
{
	public static class DeserializationExtensions
	{
		public static List<T> toList<T>(this IList self)
		{
			List<T> list = new List<T>();
			foreach (Dictionary<string, object> item in self)
			{
				list.Add(item.toClass<T>());
			}
			return list;
		}

		public static T toClass<T>(this IDictionary self)
		{
			object obj = Activator.CreateInstance(typeof(T));
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(P31DeserializeableFieldAttribute), inherit: true);
				foreach (object obj2 in customAttributes)
				{
					P31DeserializeableFieldAttribute p31DeserializeableFieldAttribute = obj2 as P31DeserializeableFieldAttribute;
					if (!self.Contains(p31DeserializeableFieldAttribute.key))
					{
						continue;
					}
					object obj3 = self[p31DeserializeableFieldAttribute.key];
					if (obj3 is IDictionary)
					{
						MethodInfo methodInfo = typeof(DeserializationExtensions).GetMethod("toClass").MakeGenericMethod(p31DeserializeableFieldAttribute.type);
						object value = methodInfo.Invoke(null, new object[1] { obj3 });
						fieldInfo.SetValue(obj, value);
						self.Remove(p31DeserializeableFieldAttribute.key);
					}
					else if (obj3 is IList)
					{
						if (!p31DeserializeableFieldAttribute.isCollection)
						{
							Debug.LogError("found an IList but the field is not a collection: " + p31DeserializeableFieldAttribute.key);
							continue;
						}
						MethodInfo methodInfo2 = typeof(DeserializationExtensions).GetMethod("toList").MakeGenericMethod(p31DeserializeableFieldAttribute.type);
						object value2 = methodInfo2.Invoke(null, new object[1] { obj3 });
						fieldInfo.SetValue(obj, value2);
						self.Remove(p31DeserializeableFieldAttribute.key);
					}
					else
					{
						fieldInfo.SetValue(obj, Convert.ChangeType(obj3, fieldInfo.FieldType));
						self.Remove(p31DeserializeableFieldAttribute.key);
					}
				}
			}
			return (T)obj;
		}

		public static Dictionary<string, object> toDictionary(this object self)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			FieldInfo[] fields = self.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(P31DeserializeableFieldAttribute), inherit: true);
				foreach (object obj in customAttributes)
				{
					P31DeserializeableFieldAttribute p31DeserializeableFieldAttribute = obj as P31DeserializeableFieldAttribute;
					if (p31DeserializeableFieldAttribute.isCollection)
					{
						IEnumerable enumerable = fieldInfo.GetValue(self) as IEnumerable;
						ArrayList arrayList = new ArrayList();
						foreach (object item in enumerable)
						{
							arrayList.Add(item.toDictionary());
						}
						dictionary[p31DeserializeableFieldAttribute.key] = arrayList;
					}
					else if (p31DeserializeableFieldAttribute.type != null)
					{
						dictionary[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self).toDictionary();
					}
					else
					{
						dictionary[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self);
					}
				}
			}
			return dictionary;
		}

		[Obsolete("Use the toDictionary method to get a proper generic Dictionary returned. Hashtables are obsolute.")]
		public static Hashtable toHashtable(this object self)
		{
			Hashtable hashtable = new Hashtable();
			FieldInfo[] fields = self.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(P31DeserializeableFieldAttribute), inherit: true);
				foreach (object obj in customAttributes)
				{
					P31DeserializeableFieldAttribute p31DeserializeableFieldAttribute = obj as P31DeserializeableFieldAttribute;
					if (p31DeserializeableFieldAttribute.isCollection)
					{
						IEnumerable enumerable = fieldInfo.GetValue(self) as IEnumerable;
						ArrayList arrayList = new ArrayList();
						foreach (object item in enumerable)
						{
							arrayList.Add(item.toHashtable());
						}
						hashtable[p31DeserializeableFieldAttribute.key] = arrayList;
					}
					else if (p31DeserializeableFieldAttribute.type != null)
					{
						hashtable[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self).toHashtable();
					}
					else
					{
						hashtable[p31DeserializeableFieldAttribute.key] = fieldInfo.GetValue(self);
					}
				}
			}
			return hashtable;
		}
	}
}
