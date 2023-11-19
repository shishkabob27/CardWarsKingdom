using System.Collections.Generic;

namespace Facebook.Unity
{
	public interface IAppLinkResult : IResult
	{
		string Url { get; }

		string TargetUrl { get; }

		string Ref { get; }

		IDictionary<string, object> Extras { get; }
	}
}
