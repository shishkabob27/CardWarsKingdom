namespace Facebook.Unity
{
	public interface IAccessTokenRefreshResult : IResult
	{
		AccessToken AccessToken { get; }
	}
}
