namespace ExitGames.Client.Photon.Chat
{
	public enum ChatDisconnectCause
	{
		None,
		DisconnectByServerUserLimit,
		ExceptionOnConnect,
		DisconnectByServer,
		TimeoutDisconnect,
		Exception,
		InvalidAuthentication,
		MaxCcuReached,
		InvalidRegion,
		OperationNotAllowedInCurrentState,
		CustomAuthenticationFailed
	}
}
