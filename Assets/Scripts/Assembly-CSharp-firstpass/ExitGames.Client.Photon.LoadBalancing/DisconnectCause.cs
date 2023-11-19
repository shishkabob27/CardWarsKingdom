namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum DisconnectCause
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
