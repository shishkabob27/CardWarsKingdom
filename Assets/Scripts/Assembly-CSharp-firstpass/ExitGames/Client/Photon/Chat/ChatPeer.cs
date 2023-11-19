using ExitGames.Client.Photon;

namespace ExitGames.Client.Photon.Chat
{
	internal class ChatPeer : PhotonPeer
	{
		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol) : base(default(ConnectionProtocol))
		{
		}

	}
}
