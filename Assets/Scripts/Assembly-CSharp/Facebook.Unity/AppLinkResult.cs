using System.Collections.Generic;

namespace Facebook.Unity
{
	internal class AppLinkResult : ResultBase, IAppLinkResult, IResult
	{
		public string Url { get; private set; }

		public string TargetUrl { get; private set; }

		public string Ref { get; private set; }

		public IDictionary<string, object> Extras { get; private set; }

		public AppLinkResult(string result)
			: base(result)
		{
			if (ResultDictionary != null)
			{
				string value;
				if (ResultDictionary.TryGetValue<string>("url", out value))
				{
					Url = value;
				}
				string value2;
				if (ResultDictionary.TryGetValue<string>("target_url", out value2))
				{
					TargetUrl = value2;
				}
				string value3;
				if (ResultDictionary.TryGetValue<string>("ref", out value3))
				{
					Ref = value3;
				}
				IDictionary<string, object> value4;
				if (ResultDictionary.TryGetValue<IDictionary<string, object>>("extras", out value4))
				{
					Extras = value4;
				}
			}
		}
	}
}
