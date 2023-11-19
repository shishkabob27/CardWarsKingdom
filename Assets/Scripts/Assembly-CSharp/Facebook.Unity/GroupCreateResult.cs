namespace Facebook.Unity
{
	internal class GroupCreateResult : ResultBase, IGroupCreateResult, IResult
	{
		public const string IDKey = "id";

		public string GroupId { get; private set; }

		public GroupCreateResult(string result)
			: base(result)
		{
			string value;
			if (ResultDictionary != null && ResultDictionary.TryGetValue<string>("id", out value))
			{
				GroupId = value;
			}
		}
	}
}
