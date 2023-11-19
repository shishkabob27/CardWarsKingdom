namespace Facebook.Unity
{
	internal class AccessTokenRefreshResult : ResultBase, IAccessTokenRefreshResult, IResult
	{
		public AccessToken AccessToken { get; private set; }

		public AccessTokenRefreshResult(string result)
			: base(result)
		{
			if (ResultDictionary != null && ResultDictionary.ContainsKey(LoginResult.AccessTokenKey))
			{
				AccessToken = Utilities.ParseAccessTokenFromResult(ResultDictionary);
			}
		}
	}
}
