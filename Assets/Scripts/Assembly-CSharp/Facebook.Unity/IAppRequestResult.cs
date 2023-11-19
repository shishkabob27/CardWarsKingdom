using System.Collections.Generic;

namespace Facebook.Unity
{
	public interface IAppRequestResult : IResult
	{
		string RequestID { get; }

		IEnumerable<string> To { get; }
	}
}
