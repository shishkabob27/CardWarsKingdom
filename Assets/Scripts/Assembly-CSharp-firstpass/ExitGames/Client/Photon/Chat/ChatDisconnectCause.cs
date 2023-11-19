namespace ExitGames.Client.Photon.Chat
{
	public enum ChatDisconnectCause
	{
		None = 0,
		DisconnectByServerUserLimit = 1,
		ExceptionOnConnect = 2,
		DisconnectByServer = 3,
		TimeoutDisconnect = 4,
		Exception = 5,
		InvalidAuthentication = 6,
		MaxCcuReached = 7,
		InvalidRegion = 8,
		OperationNotAllowedInCurrentState = 9,
		CustomAuthenticationFailed = 10,
	}
}
