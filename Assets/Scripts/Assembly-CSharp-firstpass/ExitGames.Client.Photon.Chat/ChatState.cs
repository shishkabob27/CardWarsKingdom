namespace ExitGames.Client.Photon.Chat
{
	public enum ChatState
	{
		Uninitialized,
		ConnectingToNameServer,
		ConnectedToNameServer,
		Authenticating,
		Authenticated,
		DisconnectingFromNameServer,
		ConnectingToFrontEnd,
		ConnectedToFrontEnd,
		DisconnectingFromFrontEnd,
		QueuedComingFromFrontEnd,
		Disconnecting,
		Disconnected
	}
}
