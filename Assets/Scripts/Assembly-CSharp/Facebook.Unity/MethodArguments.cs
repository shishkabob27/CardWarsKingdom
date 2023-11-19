using System;
using System.Collections.Generic;
using Facebook.MiniJSON;

namespace Facebook.Unity
{
	internal class MethodArguments
	{
		private IDictionary<string, object> arguments = new Dictionary<string, object>();

		public MethodArguments()
			: this(new Dictionary<string, object>())
		{
		}

		public MethodArguments(MethodArguments methodArgs)
			: this(methodArgs.arguments)
		{
		}

		private MethodArguments(IDictionary<string, object> arguments)
		{
			this.arguments = arguments;
		}

		public void AddPrimative<T>(string argumentName, T value) where T : struct
		{
			arguments[argumentName] = value;
		}

		public void AddNullablePrimitive<T>(string argumentName, T? nullable) where T : struct
		{
			if (nullable.HasValue && nullable.HasValue)
			{
				arguments[argumentName] = nullable.Value;
			}
		}

		public void AddString(string argumentName, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				arguments[argumentName] = value;
			}
		}

		public void AddCommaSeparatedList(string argumentName, IEnumerable<string> value)
		{
			if (value != null)
			{
				arguments[argumentName] = value.ToCommaSeparateList();
			}
		}

		public void AddDictionary(string argumentName, IDictionary<string, object> dict)
		{
			if (dict != null)
			{
				arguments[argumentName] = ToStringDict(dict);
			}
		}

		public void AddList<T>(string argumentName, IEnumerable<T> list)
		{
			if (list != null)
			{
				arguments[argumentName] = list;
			}
		}

		public void AddUri(string argumentName, Uri uri)
		{
			if (uri != null && !string.IsNullOrEmpty(uri.AbsoluteUri))
			{
				arguments[argumentName] = uri.ToString();
			}
		}

		public string ToJsonString()
		{
			return Json.Serialize(arguments);
		}

		private static Dictionary<string, string> ToStringDict(IDictionary<string, object> dict)
		{
			if (dict == null)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item in dict)
			{
				dictionary[item.Key] = item.Value.ToString();
			}
			return dictionary;
		}
	}
}
