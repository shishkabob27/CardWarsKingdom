using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;

namespace Facebook.Unity
{
	internal class GraphResult : ResultBase, IGraphResult, IResult
	{
		public IList<object> ResultList { get; private set; }

		public Texture2D Texture { get; private set; }

		internal GraphResult(WWW result)
			: base(result.text, result.error, false)
		{
			Init(RawResult);
			if (result.error == null)
			{
				Texture = result.texture;
			}
		}

		private void Init(string rawResult)
		{
			if (string.IsNullOrEmpty(rawResult))
			{
				return;
			}
			object obj = Json.Deserialize(RawResult);
			IDictionary<string, object> dictionary = obj as IDictionary<string, object>;
			if (dictionary != null)
			{
				ResultDictionary = dictionary;
				return;
			}
			IList<object> list = obj as IList<object>;
			if (list != null)
			{
				ResultList = list;
			}
		}
	}
}
