using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt.json;

namespace com.amazon.device.iap.cpt
{
	public sealed class RequestOutput : Jsonable
	{
		public string RequestId { get; set; }

		public string ToJson()
		{
			//Discarded unreachable code: IL_0013, IL_0025
			try
			{
				Dictionary<string, object> objectDictionary = GetObjectDictionary();
				return Json.Serialize(objectDictionary);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while Jsoning", inner);
			}
		}

		public override Dictionary<string, object> GetObjectDictionary()
		{
			//Discarded unreachable code: IL_001e, IL_0030
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("requestId", RequestId);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static RequestOutput CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			//Discarded unreachable code: IL_0040, IL_0052
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				RequestOutput requestOutput = new RequestOutput();
				if (jsonMap.ContainsKey("requestId"))
				{
					requestOutput.RequestId = (string)jsonMap["requestId"];
				}
				return requestOutput;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static RequestOutput CreateFromJson(string jsonMessage)
		{
			//Discarded unreachable code: IL_001e, IL_0030
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				return CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
		}

		public static Dictionary<string, RequestOutput> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, RequestOutput> dictionary = new Dictionary<string, RequestOutput>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				RequestOutput value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<RequestOutput> ListFromJson(List<object> array)
		{
			List<RequestOutput> list = new List<RequestOutput>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}
