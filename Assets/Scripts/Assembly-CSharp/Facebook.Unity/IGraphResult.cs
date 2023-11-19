using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
	public interface IGraphResult : IResult
	{
		IList<object> ResultList { get; }

		Texture2D Texture { get; }
	}
}
