using System;

namespace ExitGames.Client.Photon
{
	public class SocketUdpNativeStatic : IPhotonSocket
	{
		public SocketUdpNativeStatic(PeerBase peerBase)
			: base(peerBase)
		{
		}

		public override bool Disconnect()
		{
			throw new NotImplementedException("This class was compiled in an assembly WITH c# sockets. Another dll must be used for native sockets.");
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			throw new NotImplementedException("This class was compiled in an assembly WITH c# sockets. Another dll must be used for native sockets.");
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			throw new NotImplementedException("This class was compiled in an assembly WITH c# sockets. Another dll must be used for native sockets.");
		}
	}
}
