using System;
using System.Collections.Generic;

namespace Prime31
{
	public sealed class P31Error
	{
		private bool _containsOnlyMessage;

		public string message { get; private set; }

		public string domain { get; private set; }

		public int code { get; private set; }

		public Dictionary<string, object> userInfo { get; private set; }

		public static P31Error errorFromJson(string json)
		{
			P31Error p31Error = new P31Error();
			if (!json.StartsWith("{"))
			{
				p31Error.message = json;
				p31Error._containsOnlyMessage = true;
				return p31Error;
			}
			if (!(Json.decode(json) is Dictionary<string, object> dictionary))
			{
				p31Error.message = "Unknown error";
			}
			else
			{
				p31Error.message = ((!dictionary.ContainsKey("message")) ? null : dictionary["message"].ToString());
				p31Error.domain = ((!dictionary.ContainsKey("domain")) ? null : dictionary["domain"].ToString());
				p31Error.code = ((!dictionary.ContainsKey("code")) ? (-1) : int.Parse(dictionary["code"].ToString()));
				p31Error.userInfo = ((!dictionary.ContainsKey("userInfo")) ? null : (dictionary["userInfo"] as Dictionary<string, object>));
			}
			return p31Error;
		}

		public override string ToString()
		{
			if (_containsOnlyMessage)
			{
				return $"[P31Error]: {message}";
			}
			try
			{
				string input = Json.encode(this);
				return $"[P31Error]: {JsonFormatter.prettyPrint(input)}";
			}
			catch (Exception)
			{
				return $"[P31Error]: {message}";
			}
		}
	}
}
