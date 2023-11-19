using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketUdp : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketUdp(PeerBase npeer)
			: base(npeer)
		{
			if (ReportDebugOfLevel(DebugLevel.ALL))
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
			}
			base.Protocol = ConnectionProtocol.Udp;
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
			lock (syncer)
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
		}

		public override bool Disconnect()
		{
			if (ReportDebugOfLevel(DebugLevel.INFO))
			{
				EnqueueDebugReturn(DebugLevel.INFO, "CSharpSocket.Disconnect()");
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
			lock (syncer)
			{
				if (sock == null || !sock.Connected)
				{
					return PhotonSocketError.Skipped;
				}
				try
				{
					sock.Send(data, 0, length, SocketFlags.None);
				}
				catch (Exception ex)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
					}
					return PhotonSocketError.Exception;
				}
			}
			return PhotonSocketError.Success;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		internal void DnsAndConnect()
		{
			IPAddress iPAddress = null;
			try
			{
				lock (syncer)
				{
					iPAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
					if (iPAddress == null)
					{
						throw new ArgumentException("Invalid IPAddress. Address: " + base.ServerAddress);
					}
					if (iPAddress.AddressFamily != AddressFamily.InterNetwork && iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new ArgumentException(string.Concat("AddressFamily '", iPAddress.AddressFamily, "' not supported. Address: ", base.ServerAddress));
					}
					sock = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
					sock.Connect(iPAddress, base.ServerPort);
					base.State = PhotonSocketState.Connected;
					peerBase.SetInitIPV6Bit(iPAddress.AddressFamily == AddressFamily.InterNetworkV6);
					peerBase.OnConnect();
				}
			}
			catch (SecurityException ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' (" + ((iPAddress == null) ? "" : iPAddress.AddressFamily.ToString()) + ") failed: " + ex.ToString());
				}
				HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' (" + ((iPAddress == null) ? "" : iPAddress.AddressFamily.ToString()) + ") failed: " + ex2.ToString());
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
			byte[] array = new byte[base.MTU];
			while (base.State == PhotonSocketState.Connected)
			{
				try
				{
					int length = sock.Receive(array);
					HandleReceivedDatagram(array, length, willBeReused: true);
				}
				catch (Exception ex)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
					{
						if (ReportDebugOfLevel(DebugLevel.ERROR))
						{
							EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", base.State, ". Server: '", base.ServerAddress, "' Exception: ", ex));
						}
						HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			Disconnect();
		}
	}
}
