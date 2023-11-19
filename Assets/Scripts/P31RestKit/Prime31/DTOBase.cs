using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Prime31
{
	public class DTOBase
	{
		public static List<T> listFromJson<T>(string json) where T : DTOBase
		{
			List<object> list = json.listFromJson();
			List<T> list2 = new List<T>();
			foreach (object item2 in list)
			{
				T item = Activator.CreateInstance<T>();
				item.setDataFromDictionary(item2 as Dictionary<string, object>);
				list2.Add(item);
			}
			return list2;
		}

		public void setDataFromJson(string json)
		{
			setDataFromDictionary(json.dictionaryFromJson());
		}

		public void setDataFromDictionary(Dictionary<string, object> dict)
		{
			Dictionary<string, Action<object>> membersWithSetters = getMembersWithSetters();
			foreach (KeyValuePair<string, object> item in dict)
			{
				if (membersWithSetters.ContainsKey(item.Key))
				{
					try
					{
						membersWithSetters[item.Key](item.Value);
					}
					catch (Exception obj)
					{
						Utils.logObject(obj);
					}
				}
			}
		}

		private bool shouldIncludeTypeWithSetters(Type type)
		{
			if (type.IsGenericType)
			{
				return false;
			}
			if (type.Namespace.StartsWith("System"))
			{
				return true;
			}
			return false;
		}

		protected Dictionary<string, Action<object>> getMembersWithSetters()
		{
			Dictionary<string, Action<object>> dictionary = new Dictionary<string, Action<object>>();
			FieldInfo[] fields = GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (shouldIncludeTypeWithSetters(fieldInfo.FieldType))
				{
					FieldInfo theInfo = fieldInfo;
					dictionary[fieldInfo.Name] = delegate(object val)
					{
						theInfo.SetValue(this, val);
					};
				}
			}
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (shouldIncludeTypeWithSetters(propertyInfo.PropertyType) && propertyInfo.CanWrite && propertyInfo.GetSetMethod() != null)
				{
					PropertyInfo theInfo2 = propertyInfo;
					dictionary[propertyInfo.Name] = delegate(object val)
					{
						theInfo2.SetValue(this, val, null);
					};
				}
			}
			return dictionary;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[{0}]:", GetType());
			FieldInfo[] fields = GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				stringBuilder.AppendFormat(", {0}: {1}", fieldInfo.Name, fieldInfo.GetValue(this));
			}
			PropertyInfo[] properties = GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				stringBuilder.AppendFormat(", {0}: {1}", propertyInfo.Name, propertyInfo.GetValue(this, null));
			}
			return stringBuilder.ToString();
		}
	}
}
