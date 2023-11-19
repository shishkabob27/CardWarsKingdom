namespace ExitGames.Client.Photon.LoadBalancing
{
	public class TypedLobbyInfo : TypedLobby
	{
		public int PlayerCount;

		public int RoomCount;

		public override string ToString()
		{
			return string.Format("TypedLobbyInfo '{0}'[{1}] rooms: {2} players: {3}", Name, Type, RoomCount, PlayerCount);
		}
	}
}
