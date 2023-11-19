using System.Collections.Generic;

namespace Facebook.Unity
{
	public interface IResult
	{
		string Error { get; }

		IDictionary<string, object> ResultDictionary { get; }

		string RawResult { get; }

		bool Cancelled { get; }
	}
}
