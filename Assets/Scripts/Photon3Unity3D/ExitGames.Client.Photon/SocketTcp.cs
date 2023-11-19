using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketTcp : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketTcp(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "SocketTcp: TCP, DotNet, Unity.");
			}
			base.Protocol = ConnectionProtocol.Tcp;
			PollReceive = false;
		}

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			if (sock != null)
			{
				try
				{
					if (sock.Connected)
					{
						sock.Close();
					}
				}
				catch (Exception ex)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
				}
			}
			sock = null;
			base.State = PhotonSocketState.Disconnected;
		}

		public override bool Connect()
		{
			if (!base.Connect())
			{
				return false;
			}
			base.State = PhotonSocketState.Connecting;
			Thread thread = new Thread(DnsAndConnect);
			thread.Name = "photon dns thread";
			thread.IsBackground = true;
			thread.Start();
			return true;
		}

		public override bool Disconnect()
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				EnqueueDebugReturn(DebugLevel.INFO, "SocketTcp.Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			lock (syncer)
			{
				if (sock != null)
				{
					try
					{
						sock.Close();
					}
					catch (Exception ex)
					{
						EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
					}
					sock = null;
				}
			}
			base.State = PhotonSocketState.Disconnected;
			return true;
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			if (!sock.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			try
			{
				sock.Send(data);
			}
			catch (Exception ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
				}
				HandleException(StatusCode.Exception);
				return PhotonSocketError.Exception;
			}
			return PhotonSocketError.Success;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		public void DnsAndConnect()
		{
			try
			{
				IPAddress ipAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
				if (ipAddress == null)
				{
					throw new ArgumentException("Invalid IPAddress. Address: " + base.ServerAddress);
				}
				if (ipAddress.AddressFamily != AddressFamily.InterNetwork && ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentException(string.Concat("AddressFamily '", ipAddress.AddressFamily, "' not supported. Address: ", base.ServerAddress));
				}
				sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				sock.NoDelay = true;
				sock.ReceiveTimeout = peerBase.DisconnectTimeout;
				sock.SendTimeout = peerBase.DisconnectTimeout;
				sock.Connect(ipAddress, base.ServerPort);
				base.State = PhotonSocketState.Connected;
				peerBase.SetInitIPV6Bit(ipAddress.AddressFamily == AddressFamily.InterNetworkV6);
				peerBase.OnConnect();
			}
			catch (SecurityException ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex.ToString());
				}
				HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex2.ToString());
				}
				HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			Thread thread = new Thread(ReceiveLoop);
			thread.Name = "photon receive thread";
			thread.IsBackground = true;
			thread.Start();
		}

		public void ReceiveLoop()
		{
			MemoryStream memoryStream = new MemoryStream(base.MTU);
			while (base.State == PhotonSocketState.Connected)
			{
				memoryStream.Position = 0L;
				memoryStream.SetLength(0L);
				try
				{
					int num = 0;
					int num2 = 0;
					byte[] array = new byte[9];
					while (num < 9)
					{
						num2 = sock.Receive(array, num, 9 - num, SocketFlags.None);
						num += num2;
						if (num2 == 0)
						{
							throw new SocketException(10054);
						}
					}
					if (array[0] == 240)
					{
						HandleReceivedDatagram(array, array.Length, willBeReused: false);
						continue;
					}
					int num3 = (array[1] << 24) | (array[2] << 16) | (array[3] << 8) | array[4];
					if (peerBase.TrafficStatsEnabled)
					{
						if (array[5] == 0)
						{
							peerBase.TrafficStatsIncoming.CountReliableOpCommand(num3);
						}
						else
						{
							peerBase.TrafficStatsIncoming.CountUnreliableOpCommand(num3);
						}
					}
					if (ReportDebugOfLevel(DebugLevel.ALL))
					{
						EnqueueDebugReturn(DebugLevel.ALL, "message length: " + num3);
					}
					memoryStream.Write(array, 7, num - 7);
					num = 0;
					num3 -= 9;
					array = new byte[num3];
					while (num < num3)
					{
						num2 = sock.Receive(array, num, num3 - num, SocketFlags.None);
						num += num2;
						if (num2 == 0)
						{
							throw new SocketException(10054);
						}
					}
					memoryStream.Write(array, 0, num);
					if (memoryStream.Length > 0)
					{
						HandleReceivedDatagram(memoryStream.ToArray(), (int)memoryStream.Length, willBeReused: false);
					}
					if (ReportDebugOfLevel(DebugLevel.ALL))
					{
						EnqueueDebugReturn(DebugLevel.ALL, "TCP < " + memoryStream.Length + ((memoryStream.Length == num3 + 2) ? " OK" : " BAD"));
					}
				}
				catch (SocketException ex)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, "Receiving failed. SocketException: " + ex.SocketErrorCode);
						}
						if (ex.SocketErrorCode == SocketError.ConnectionReset || ex.SocketErrorCode == SocketError.ConnectionAborted)
						{
							HandleException(StatusCode.DisconnectByServer);
						}
						else
						{
							HandleException(StatusCode.ExceptionOnReceive);
						}
					}
				}
				catch (Exception ex2)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex2));
						}
						HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			Disconnect();
		}
	}
}
