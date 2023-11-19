using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class AppRequestResult : ResultBase, IAppRequestResult, IResult
	{
		public const string RequestIDKey = "request";

		public const string ToKey = "to";

		public string RequestID { get; private set; }

		public IEnumerable<string> To { get; private set; }

		public AppRequestResult(string result)
			: base(result)
		{
			if (ResultDictionary == null)
			{
				return;
			}
			string value;
			if (ResultDictionary.TryGetValue<string>("request", out value))
			{
				RequestID = value;
			}
			string value2;
			if (ResultDictionary.TryGetValue<string>("to", out value2))
			{
				To = value2.Split(',');
			}
			else
			{
				IEnumerable<object> value3;
				if (!ResultDictionary.TryGetValue<IEnumerable<object>>("to", out value3))
				{
					return;
				}
				List<string> list = new List<string>();
				foreach (object item in value3)
				{
					string text = item as string;
					if (text != null)
					{
						list.Add(text);
					}
				}
				To = list;
			}
		}
	}
}
