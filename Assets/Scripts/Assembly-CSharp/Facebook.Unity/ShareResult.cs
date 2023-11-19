namespace Facebook.Unity
{
	internal class ShareResult : ResultBase, IResult, IShareResult
	{
		public string PostId { get; private set; }

		internal ShareResult(string result)
			: base(result)
		{
			object value;
			if (ResultDictionary != null && ResultDictionary.TryGetValue("id", out value))
			{
				PostId = value as string;
			}
		}
	}
}
