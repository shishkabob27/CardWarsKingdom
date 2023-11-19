// ExitGames.Client.Photon.IPhotonSocket
using System;
using System.Net;
using System.Net.Sockets;
using ExitGames.Client.Photon;

public abstract class IPhotonSocket
{
	protected internal PeerBase peerBase;

	public bool PollReceive;

	protected IPhotonPeerListener Listener
	{
		get
		{
			return peerBase.Listener;
		}
	}

	public ConnectionProtocol Protocol { get; protected set; }

	public PhotonSocketState State { get; protected set; }

	public string ServerAddress { get; protected set; }

	public int ServerPort { get; protected set; }

	public string UrlProtocol { get; protected set; }

	public string UrlPath { get; protected set; }

	public bool Connected
	{
		get
		{
			return State == PhotonSocketState.Connected;
		}
	}

	public int MTU
	{
		get
		{
			return peerBase.mtu;
		}
	}

	public IPhotonSocket(PeerBase peerBase)
	{
		if (peerBase == null)
		{
			throw new Exception("Can't init without peer");
		}
		this.peerBase = peerBase;
	}

	public virtual bool Connect()
	{
		if (State != 0)
		{
			if ((int)peerBase.debugOut >= 1)
			{
				peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: connection in State: " + State);
			}
			return false;
		}
		if (peerBase == null || Protocol != peerBase.usedProtocol)
		{
			return false;
		}
		string address;
		ushort port;
		string urlProtocol;
		string urlPath;
		if (!TryParseAddress(peerBase.ServerAddress, out address, out port, out urlProtocol, out urlPath))
		{
			if ((int)peerBase.debugOut >= 1)
			{
				peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Failed parsing address: " + peerBase.ServerAddress);
			}
			return false;
		}
		ServerAddress = address;
		ServerPort = port;
		UrlProtocol = urlProtocol;
		UrlPath = urlPath;
		return true;
	}

	public abstract bool Disconnect();

	public abstract PhotonSocketError Send(byte[] data, int length);

	public abstract PhotonSocketError Receive(out byte[] data);

	public void HandleReceivedDatagram(byte[] inBuffer, int length, bool willBeReused)
	{
		if (peerBase.NetworkSimulationSettings.IsSimulationEnabled)
		{
			if (willBeReused)
			{
				byte[] inBufferCopy = new byte[length];
				Buffer.BlockCopy(inBuffer, 0, inBufferCopy, 0, length);
				peerBase.ReceiveNetworkSimulated(delegate
				{
					peerBase.ReceiveIncomingCommands(inBufferCopy, length);
				});
			}
			else
			{
				peerBase.ReceiveNetworkSimulated(delegate
				{
					peerBase.ReceiveIncomingCommands(inBuffer, length);
				});
			}
		}
		else
		{
			peerBase.ReceiveIncomingCommands(inBuffer, length);
		}
	}

	public bool ReportDebugOfLevel(DebugLevel levelOfMessage)
	{
		return (int)peerBase.debugOut >= (int)levelOfMessage;
	}

	public void EnqueueDebugReturn(DebugLevel debugLevel, string message)
	{
		peerBase.EnqueueDebugReturn(debugLevel, message);
	}

	protected internal void HandleException(StatusCode statusCode)
	{
		State = PhotonSocketState.Disconnecting;
		peerBase.EnqueueStatusCallback(statusCode);
		peerBase.EnqueueActionForDispatch(delegate
		{
			peerBase.Disconnect();
		});
	}

	protected internal bool TryParseAddress(string url, out string address, out ushort port, out string urlProtocol, out string urlPath)
	{
		address = string.Empty;
		port = 0;
		urlProtocol = string.Empty;
		urlPath = string.Empty;
		string text = url;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		int num = text.IndexOf("://");
		if (num >= 0)
		{
			urlProtocol = text.Substring(0, num);
			text = text.Substring(num + 3);
		}
		num = text.IndexOf("/");
		if (num >= 0)
		{
			urlPath = text.Substring(num);
			text = text.Substring(0, num);
		}
		num = text.LastIndexOf(':');
		if (num < 0)
		{
			return false;
		}
		if (text.IndexOf(':') != num && (!text.Contains("[") || !text.Contains("]")))
		{
			return false;
		}
		address = text.Substring(0, num);
		string s = text.Substring(num + 1);
		return ushort.TryParse(s, out port);
	}

	protected internal static IPAddress GetIpAddress(string serverIp)
	{
		IPAddress address = null;
		if (IPAddress.TryParse(serverIp, out address))
		{
			return address;
		}
		IPHostEntry hostEntry = Dns.GetHostEntry(serverIp);
		IPAddress[] addressList = hostEntry.AddressList;
		IPAddress[] array = addressList;
		foreach (IPAddress iPAddress in array)
		{
			if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return iPAddress;
			}
		}
		IPAddress[] array2 = addressList;
		foreach (IPAddress iPAddress2 in array2)
		{
			if (iPAddress2.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return iPAddress2;
			}
		}
		return null;
	}
}
