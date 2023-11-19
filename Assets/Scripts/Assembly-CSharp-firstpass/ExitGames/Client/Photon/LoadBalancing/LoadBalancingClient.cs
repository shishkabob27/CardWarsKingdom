using ExitGames.Client.Photon;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class LoadBalancingClient
	{
		public LoadBalancingClient(ConnectionProtocol protocol)
		{
		}

		public string NameServerHost;
		public string NameServerHttp;
		public bool EnableLobbyStatistics;
	}
}
