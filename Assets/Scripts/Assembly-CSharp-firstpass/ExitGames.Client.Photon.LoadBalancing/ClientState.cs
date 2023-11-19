namespace ExitGames.Client.Photon.LoadBalancing
{
	public enum ClientState
	{
		Uninitialized,
		ConnectingToMasterserver,
		ConnectedToMaster,
		Queued,
		Authenticated,
		JoinedLobby,
		DisconnectingFromMasterserver,
		ConnectingToGameserver,
		ConnectedToGameserver,
		Joining,
		Joined,
		Leaving,
		Left,
		DisconnectingFromGameserver,
		QueuedComingFromGameserver,
		Disconnecting,
		Disconnected,
		ConnectingToNameServer,
		ConnectedToNameServer,
		Authenticating,
		DisconnectingFromNameServer
	}
}
