using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class PhotonPeer
	{
		public const bool NoSocket = false;

		internal PeerBase peerBase;

		private readonly object SendOutgoingLockObject = new object();

		private readonly object DispatchLockObject = new object();

		private readonly object EnqueueLock = new object();

		public Type SocketImplementation
		{
			get
			{
				return peerBase.SocketImplementation;
			}
			set
			{
				peerBase.SocketImplementation = value;
			}
		}

		public DebugLevel DebugOut
		{
			get
			{
				return peerBase.debugOut;
			}
			set
			{
				peerBase.debugOut = value;
			}
		}

		public IPhotonPeerListener Listener
		{
			get
			{
				return peerBase.Listener;
			}
			protected set
			{
				peerBase.Listener = value;
			}
		}

		public long BytesIn => peerBase.BytesIn;

		public long BytesOut => peerBase.BytesOut;

		public int ByteCountCurrentDispatch => peerBase.ByteCountCurrentDispatch;

		public string CommandInfoCurrentDispatch => (peerBase.CommandInCurrentDispatch != null) ? peerBase.CommandInCurrentDispatch.ToString() : string.Empty;

		public int ByteCountLastOperation => peerBase.ByteCountLastOperation;

		public bool TrafficStatsEnabled
		{
			get
			{
				return peerBase.TrafficStatsEnabled;
			}
			set
			{
				peerBase.TrafficStatsEnabled = value;
			}
		}

		public long TrafficStatsElapsedMs => peerBase.TrafficStatsEnabledTime;

		public TrafficStats TrafficStatsIncoming => peerBase.TrafficStatsIncoming;

		public TrafficStats TrafficStatsOutgoing => peerBase.TrafficStatsOutgoing;

		public TrafficStatsGameLevel TrafficStatsGameLevel => peerBase.TrafficStatsGameLevel;

		public int CommandLogSize
		{
			get
			{
				return peerBase.CommandLogSize;
			}
			set
			{
				peerBase.CommandLogSize = value;
				peerBase.CommandLogResize();
			}
		}

		public byte QuickResendAttempts
		{
			get
			{
				return peerBase.QuickResendAttempts;
			}
			set
			{
				peerBase.QuickResendAttempts = (byte)((value > 4) ? 4 : value);
			}
		}

		public PeerStateValue PeerState
		{
			get
			{
				if (peerBase.peerConnectionState == PeerBase.ConnectionStateValue.Connected && !peerBase.ApplicationIsInitialized)
				{
					return PeerStateValue.InitializingApplication;
				}
				return (PeerStateValue)peerBase.peerConnectionState;
			}
		}

		public string PeerID => peerBase.PeerID;

		public int CommandBufferSize => peerBase.commandBufferSize;

		public int RhttpMinConnections
		{
			get
			{
				return peerBase.rhttpMinConnections;
			}
			set
			{
				if (value >= 8)
				{
					if ((int)DebugOut >= 2)
					{
						Listener.DebugReturn(DebugLevel.WARNING, "Forcing RhttpMinConnections=7 the currently max supported value.");
					}
					peerBase.rhttpMinConnections = 7;
				}
				else
				{
					peerBase.rhttpMinConnections = value;
				}
			}
		}

		public int RhttpMaxConnections
		{
			get
			{
				return peerBase.rhttpMaxConnections;
			}
			set
			{
				peerBase.rhttpMaxConnections = value;
			}
		}

		public int LimitOfUnreliableCommands
		{
			get
			{
				return peerBase.limitOfUnreliableCommands;
			}
			set
			{
				peerBase.limitOfUnreliableCommands = value;
			}
		}

		public int QueuedIncomingCommands => peerBase.QueuedIncomingCommandsCount;

		public int QueuedOutgoingCommands => peerBase.QueuedOutgoingCommandsCount;

		public byte ChannelCount
		{
			get
			{
				return peerBase.ChannelCount;
			}
			set
			{
				if (value == 0 || PeerState != 0)
				{
					throw new Exception("ChannelCount can only be set while disconnected and must be > 0.");
				}
				peerBase.ChannelCount = value;
			}
		}

		public bool CrcEnabled
		{
			get
			{
				return peerBase.crcEnabled;
			}
			set
			{
				if (peerBase.peerConnectionState != 0)
				{
					throw new Exception("CrcEnabled can only be set while disconnected.");
				}
				peerBase.crcEnabled = value;
			}
		}

		public int PacketLossByCrc => peerBase.packetLossByCrc;

		public int PacketLossByChallenge => peerBase.packetLossByChallenge;

		public int ResentReliableCommands => (UsedProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)peerBase).reliableCommandsRepeated : 0;

		public int WarningSize
		{
			get
			{
				return peerBase.warningSize;
			}
			set
			{
				peerBase.warningSize = value;
			}
		}

		public int SentCountAllowance
		{
			get
			{
				return peerBase.sentCountAllowance;
			}
			set
			{
				peerBase.sentCountAllowance = value;
			}
		}

		public int TimePingInterval
		{
			get
			{
				return peerBase.timePingInterval;
			}
			set
			{
				peerBase.timePingInterval = value;
			}
		}

		public int DisconnectTimeout
		{
			get
			{
				return peerBase.DisconnectTimeout;
			}
			set
			{
				peerBase.DisconnectTimeout = value;
			}
		}

		public int ServerTimeInMilliSeconds => peerBase.serverTimeOffsetIsAvailable ? (peerBase.serverTimeOffset + SupportClass.GetTickCount()) : 0;

		public int ConnectionTime => peerBase.timeInt;

		public int LastSendAckTime => peerBase.timeLastSendAck;

		public int LastSendOutgoingTime => peerBase.timeLastSendOutgoing;

		[Obsolete("Should be replaced by: SupportClass.GetTickCount(). Internally this is used, too.")]
		public int LocalTimeInMilliSeconds => SupportClass.GetTickCount();

		public SupportClass.IntegerMillisecondsDelegate LocalMsTimestampDelegate
		{
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("LocalMsTimestampDelegate only settable while disconnected. State: " + PeerState);
				}
				SupportClass.IntegerMilliseconds = value;
			}
		}

		public int RoundTripTime => peerBase.roundTripTime;

		public int RoundTripTimeVariance => peerBase.roundTripTimeVariance;

		public int TimestampOfLastSocketReceive => peerBase.timestampOfLastReceive;

		public string ServerAddress
		{
			get
			{
				return peerBase.ServerAddress;
			}
			set
			{
				if ((int)DebugOut >= 1)
				{
					Listener.DebugReturn(DebugLevel.ERROR, "Failed to set ServerAddress. Can be set only when using HTTP.");
				}
			}
		}

		public ConnectionProtocol UsedProtocol => peerBase.usedProtocol;

		public virtual bool IsSimulationEnabled
		{
			get
			{
				return NetworkSimulationSettings.IsSimulationEnabled;
			}
			set
			{
				if (value == NetworkSimulationSettings.IsSimulationEnabled)
				{
					return;
				}
				lock (SendOutgoingLockObject)
				{
					NetworkSimulationSettings.IsSimulationEnabled = value;
				}
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings => peerBase.NetworkSimulationSettings;

		public static int OutgoingStreamBufferSize
		{
			get
			{
				return PeerBase.outgoingStreamBufferSize;
			}
			set
			{
				PeerBase.outgoingStreamBufferSize = value;
			}
		}

		public int MaximumTransferUnit
		{
			get
			{
				return peerBase.mtu;
			}
			set
			{
				if (PeerState != 0)
				{
					throw new Exception("MaximumTransferUnit is only settable while disconnected. State: " + PeerState);
				}
				if (value < 576)
				{
					value = 576;
				}
				peerBase.mtu = value;
			}
		}

		public bool IsEncryptionAvailable => peerBase.isEncryptionAvailable;

		public bool IsSendingOnlyAcks
		{
			get
			{
				return peerBase.IsSendingOnlyAcks;
			}
			set
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.IsSendingOnlyAcks = value;
				}
			}
		}

		public void TrafficStatsReset()
		{
			peerBase.InitializeTrafficStats();
			peerBase.TrafficStatsEnabled = true;
		}

		public string CommandLogToString()
		{
			return peerBase.CommandLogToString();
		}

		public PhotonPeer(ConnectionProtocol protocolType)
		{
			switch (protocolType)
			{
			case ConnectionProtocol.Tcp:
				peerBase = new TPeer();
				peerBase.usedProtocol = protocolType;
				break;
			case ConnectionProtocol.Udp:
				peerBase = new EnetPeer();
				peerBase.usedProtocol = protocolType;
				break;
			default:
				if (protocolType != ConnectionProtocol.WebSocketSecure)
				{
					break;
				}
				goto case ConnectionProtocol.WebSocket;
			case ConnectionProtocol.WebSocket:
				peerBase = new TPeer
				{
					DoFraming = false
				};
				peerBase.usedProtocol = protocolType;
				break;
			}
		}

		public PhotonPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType)
			: this(protocolType)
		{
			Listener = listener;
		}

		[Obsolete("Use the constructor with ConnectionProtocol instead.")]
		public PhotonPeer(IPhotonPeerListener listener)
			: this(listener, ConnectionProtocol.Udp)
		{
		}

		[Obsolete("Use the constructor with ConnectionProtocol instead.")]
		public PhotonPeer(IPhotonPeerListener listener, bool useTcp)
			: this(listener, useTcp ? ConnectionProtocol.Tcp : ConnectionProtocol.Udp)
		{
		}

		public virtual bool Connect(string serverAddress, string applicationName)
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					return peerBase.Connect(serverAddress, applicationName);
				}
			}
		}

		public virtual void Disconnect()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.Disconnect();
				}
			}
		}

		public virtual void StopThread()
		{
			lock (DispatchLockObject)
			{
				lock (SendOutgoingLockObject)
				{
					peerBase.StopConnection();
				}
			}
		}

		public virtual void FetchServerTimestamp()
		{
			peerBase.FetchServerTimestamp();
		}

		public bool EstablishEncryption()
		{
			return peerBase.ExchangeKeysForEncryption();
		}

		public virtual void Service()
		{
			while (DispatchIncomingCommands())
			{
			}
			while (SendOutgoingCommands())
			{
			}
		}

		public virtual bool SendOutgoingCommands()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendOutgoingCommands();
			}
		}

		public virtual bool SendAcksOnly()
		{
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			lock (SendOutgoingLockObject)
			{
				return peerBase.SendAcksOnly();
			}
		}

		public virtual bool DispatchIncomingCommands()
		{
			peerBase.ByteCountCurrentDispatch = 0;
			if (TrafficStatsEnabled)
			{
				TrafficStatsGameLevel.DispatchIncomingCommandsCalled();
			}
			lock (DispatchLockObject)
			{
				return peerBase.DispatchIncomingCommands();
			}
		}

		public string VitalStatsToString(bool all)
		{
			if (TrafficStatsGameLevel == null)
			{
				return "Stats not available. Use PhotonPeer.TrafficStatsEnabled.";
			}
			if (!all)
			{
				return string.Format("Rtt(variance): {0}({1}). Ms since last receive: {2}. Stats elapsed: {4}sec.\n{3}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, TrafficStatsGameLevel.ToStringVitalStats(), TrafficStatsElapsedMs / 1000);
			}
			return string.Format("Rtt(variance): {0}({1}). Ms since last receive: {2}. Stats elapsed: {6}sec.\n{3}\n{4}\n{5}", RoundTripTime, RoundTripTimeVariance, SupportClass.GetTickCount() - TimestampOfLastSocketReceive, TrafficStatsGameLevel.ToStringVitalStats(), TrafficStatsIncoming.ToString(), TrafficStatsOutgoing.ToString(), TrafficStatsElapsedMs / 1000);
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable)
		{
			return OpCustom(customOpCode, customOpParameters, sendReliable, 0);
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId)
		{
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(customOpParameters, customOpCode, sendReliable, channelId, encrypted: false);
			}
		}

		public virtual bool OpCustom(byte customOpCode, Dictionary<byte, object> customOpParameters, bool sendReliable, byte channelId, bool encrypt)
		{
			if (encrypt && !IsEncryptionAvailable)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(customOpParameters, customOpCode, sendReliable, channelId, encrypt);
			}
		}

		public virtual bool OpCustom(OperationRequest operationRequest, bool sendReliable, byte channelId, bool encrypt)
		{
			if (encrypt && !IsEncryptionAvailable)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			lock (EnqueueLock)
			{
				return peerBase.EnqueueOperation(operationRequest.Parameters, operationRequest.OperationCode, sendReliable, channelId, encrypt);
			}
		}

		public static bool RegisterType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}

		public static bool RegisterType(Type customType, byte code, SerializeStreamMethod serializeMethod, DeserializeStreamMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}
	}
}
