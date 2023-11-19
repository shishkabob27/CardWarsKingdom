using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon.Chat
{
	internal class ChatPeer : PhotonPeer
	{
		public const string NameServerHost = "ns.exitgames.com";

		public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};

		public string NameServerAddress
		{
			get
			{
				return GetNameServerAddress();
			}
		}

		internal virtual bool IsProtocolSecure
		{
			get
			{
				return base.UsedProtocol == ConnectionProtocol.WebSocketSecure;
			}
		}

		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol)
			: base(listener, protocol)
		{
			if (protocol == ConnectionProtocol.WebSocket || protocol == ConnectionProtocol.WebSocketSecure)
			{
				UnityEngine.Debug.Log("Using SocketWebTcp");
				base.SocketImplementation = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp");
			}
		}

		private string GetNameServerAddress()
		{
			ConnectionProtocol usedProtocol = base.UsedProtocol;
			int value = 0;
			ProtocolToNameServerPort.TryGetValue(usedProtocol, out value);
			string arg = string.Empty;
			switch (usedProtocol)
			{
			case ConnectionProtocol.WebSocket:
				arg = "ws://";
				break;
			case ConnectionProtocol.WebSocketSecure:
				arg = "wss://";
				break;
			}
			return string.Format("{0}{1}:{2}", arg, "ns.exitgames.com", value);
		}

		public bool Connect()
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "Connecting to nameserver " + NameServerAddress);
			return Connect(NameServerAddress, "NameServer");
		}

		public bool AuthenticateOnNameServer(string appId, string appVersion, string region, AuthenticationValues authValues)
		{
			if ((int)base.DebugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[220] = appVersion;
			dictionary[224] = appId;
			dictionary[210] = region;
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary[225] = authValues.UserId;
				}
				if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
				{
					dictionary[217] = (byte)authValues.AuthType;
					if (!string.IsNullOrEmpty(authValues.Token))
					{
						dictionary[221] = authValues.Token;
					}
					else
					{
						if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
						{
							dictionary[216] = authValues.AuthGetParameters;
						}
						if (authValues.AuthPostData != null)
						{
							dictionary[214] = authValues.AuthPostData;
						}
					}
				}
			}
			return OpCustom(230, dictionary, true, 0, base.IsEncryptionAvailable);
		}
	}
}
