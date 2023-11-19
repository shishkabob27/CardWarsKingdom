using System;
using System.Net.Sockets;

namespace ExitGames.Client.Photon
{
	public class PingMono : PhotonPing
	{
		private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		public override bool StartPing(string ip)
		{
			Init();
			try
			{
				sock.ReceiveTimeout = 5000;
				sock.Connect(ip, 5055);
				PingBytes[PingBytes.Length - 1] = PingId;
				sock.Send(PingBytes);
				PingBytes[PingBytes.Length - 1] = (byte)(PingId - 1);
			}
			catch (Exception value)
			{
				sock = null;
				Console.WriteLine(value);
			}
			return false;
		}

		public override bool Done()
		{
			if (GotResult || sock == null)
			{
				return true;
			}
			if (sock.Available <= 0)
			{
				return false;
			}
			int num = sock.Receive(PingBytes, SocketFlags.None);
			if (PingBytes[PingBytes.Length - 1] != PingId || num != PingLength)
			{
				DebugString += " ReplyMatch is false! ";
			}
			Successful = num == PingBytes.Length && PingBytes[PingBytes.Length - 1] == PingId;
			GotResult = true;
			return true;
		}

		public override void Dispose()
		{
			try
			{
				sock.Close();
			}
			catch
			{
			}
			sock = null;
		}
	}
}
