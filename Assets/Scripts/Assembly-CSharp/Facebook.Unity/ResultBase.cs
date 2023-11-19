using System;
using System.Collections.Generic;
using Facebook.MiniJSON;

namespace Facebook.Unity
{
	internal abstract class ResultBase : IInternalResult, IResult
	{
		public virtual string Error { get; protected set; }

		public virtual IDictionary<string, object> ResultDictionary { get; protected set; }

		public virtual string RawResult { get; protected set; }

		public virtual bool Cancelled { get; protected set; }

		public virtual string CallbackId { get; protected set; }

		internal ResultBase(string result)
		{
			string error = null;
			bool cancelled = false;
			string callbackId = null;
			if (!string.IsNullOrEmpty(result))
			{
				Dictionary<string, object> dictionary = Json.Deserialize(result) as Dictionary<string, object>;
				if (dictionary != null)
				{
					ResultDictionary = dictionary;
					error = GetErrorValue(dictionary);
					cancelled = GetCancelledValue(dictionary);
					callbackId = GetCallbackId(dictionary);
				}
			}
			Init(result, error, cancelled, callbackId);
		}

		internal ResultBase(string result, string error, bool cancelled)
		{
			Init(result, error, cancelled, null);
		}

		public override string ToString()
		{
			return string.Format("[BaseResult: Error={0}, Result={1}, RawResult={2}, Cancelled={3}]", Error, ResultDictionary, RawResult, Cancelled);
		}

		protected void Init(string result, string error, bool cancelled, string callbackId)
		{
			RawResult = result;
			Cancelled = cancelled;
			Error = error;
			CallbackId = callbackId;
		}

		private static string GetErrorValue(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return null;
			}
			string value;
			if (result.TryGetValue<string>("error", out value))
			{
				return value;
			}
			return null;
		}

		private static bool GetCancelledValue(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return false;
			}
			object value;
			if (result.TryGetValue("cancelled", out value))
			{
				bool? flag = value as bool?;
				if (flag.HasValue)
				{
					return flag.HasValue && flag.Value;
				}
				string text = value as string;
				if (text != null)
				{
					return Convert.ToBoolean(text);
				}
				int? num = value as int?;
				if (num.HasValue)
				{
					return num.HasValue && num.Value != 0;
				}
			}
			return false;
		}

		private static string GetCallbackId(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return null;
			}
			string value;
			if (result.TryGetValue<string>("callback_id", out value))
			{
				return value;
			}
			return null;
		}
	}
}
