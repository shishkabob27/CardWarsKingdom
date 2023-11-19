namespace ExitGames.Client.Photon.LoadBalancing
{
	public class GamePropertyKey
	{
		public const byte MaxPlayers = byte.MaxValue;

		public const byte IsVisible = 254;

		public const byte IsOpen = 253;

		public const byte PlayerCount = 252;

		public const byte Removed = 251;

		public const byte PropsListedInLobby = 250;

		public const byte CleanupCacheOnLeave = 249;

		public const byte MasterClientId = 248;

		public const byte ExpectedUsers = 247;
	}
}
