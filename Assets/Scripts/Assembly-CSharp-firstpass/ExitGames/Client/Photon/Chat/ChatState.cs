namespace ExitGames.Client.Photon.Chat
{
	public enum ChatState
	{
		Uninitialized = 0,
		ConnectingToNameServer = 1,
		ConnectedToNameServer = 2,
		Authenticating = 3,
		Authenticated = 4,
		DisconnectingFromNameServer = 5,
		ConnectingToFrontEnd = 6,
		ConnectedToFrontEnd = 7,
		DisconnectingFromFrontEnd = 8,
		QueuedComingFromFrontEnd = 9,
		Disconnecting = 10,
		Disconnected = 11,
	}
}
