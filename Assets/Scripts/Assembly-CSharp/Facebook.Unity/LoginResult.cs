namespace Facebook.Unity
{
	internal class LoginResult : ResultBase, ILoginResult, IResult
	{
		public const string LastRefreshKey = "last_refresh";

		public static readonly string UserIdKey = ((!Constants.IsWeb) ? "user_id" : "userID");

		public static readonly string ExpirationTimestampKey = ((!Constants.IsWeb) ? "expiration_timestamp" : "expiresIn");

		public static readonly string PermissionsKey = ((!Constants.IsWeb) ? "permissions" : "grantedScopes");

		public static readonly string AccessTokenKey = ((!Constants.IsWeb) ? "access_token" : "accessToken");

		public AccessToken AccessToken { get; private set; }

		internal LoginResult(string response)
			: base(response)
		{
			if (ResultDictionary != null && ResultDictionary.ContainsKey(AccessTokenKey))
			{
				AccessToken = Utilities.ParseAccessTokenFromResult(ResultDictionary);
			}
		}
	}
}
