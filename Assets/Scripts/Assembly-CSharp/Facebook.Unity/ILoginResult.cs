namespace Facebook.Unity
{
	public interface ILoginResult : IResult
	{
		AccessToken AccessToken { get; }
	}
}
