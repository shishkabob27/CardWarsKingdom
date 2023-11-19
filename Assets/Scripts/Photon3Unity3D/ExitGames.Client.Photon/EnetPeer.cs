// ExitGames.Client.Photon.EnetPeer
using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;

internal class EnetPeer : PeerBase
{
	private const int CRC_LENGTH = 4;

	private Dictionary<byte, EnetChannel> channels = new Dictionary<byte, EnetChannel>();

	private List<NCommand> sentReliableCommands = new List<NCommand>();

	private Queue<NCommand> outgoingAcknowledgementsList = new Queue<NCommand>();

	internal readonly int windowSize = 128;

	private byte udpCommandCount;

	private byte[] udpBuffer;

	private int udpBufferIndex;

	internal int challenge;

	internal int reliableCommandsRepeated;

	internal int reliableCommandsSent;

	internal int serverSentTime;

	internal static readonly byte[] udpHeader0xF3 = new byte[2] { 243, 2 };

	internal static readonly byte[] messageHeader = udpHeader0xF3;

	private byte[] initData = null;

	private EnetChannel[] channelArray = new EnetChannel[0];

	private Queue<int> commandsToRemove = new Queue<int>();

	internal override int QueuedIncomingCommandsCount
	{
		get
		{
			int num = 0;
			lock (channels)
			{
				foreach (EnetChannel value in channels.Values)
				{
					num += value.incomingReliableCommandsList.Count;
					num += value.incomingUnreliableCommandsList.Count;
				}
			}
			return num;
		}
	}

	internal override int QueuedOutgoingCommandsCount
	{
		get
		{
			int num = 0;
			lock (channels)
			{
				foreach (EnetChannel value in channels.Values)
				{
					num += value.outgoingReliableCommandsList.Count;
					num += value.outgoingUnreliableCommandsList.Count;
				}
			}
			return num;
		}
	}

	internal EnetPeer()
	{
		PeerBase.peerCount++;
		InitOnce();
		TrafficPackageHeaderSize = 12;
	}

	internal EnetPeer(IPhotonPeerListener listener)
		: this()
	{
		base.Listener = listener;
	}

	internal override void InitPeerBase()
	{
		base.InitPeerBase();
		peerID = -1;
		challenge = SupportClass.ThreadSafeRandom.Next();
		udpBuffer = new byte[mtu];
		reliableCommandsSent = 0;
		reliableCommandsRepeated = 0;
		lock (channels)
		{
			channels = new Dictionary<byte, EnetChannel>();
		}
		lock (channels)
		{
			channels[byte.MaxValue] = new EnetChannel(byte.MaxValue, commandBufferSize);
			for (byte b = 0; b < ChannelCount; b = (byte)(b + 1))
			{
				channels[b] = new EnetChannel(b, commandBufferSize);
			}
			channelArray = new EnetChannel[ChannelCount + 1];
			int num = 0;
			foreach (EnetChannel value in channels.Values)
			{
				channelArray[num++] = value;
			}
		}
		lock (sentReliableCommands)
		{
			sentReliableCommands = new List<NCommand>(commandBufferSize);
		}
		lock (outgoingAcknowledgementsList)
		{
			outgoingAcknowledgementsList = new Queue<NCommand>(commandBufferSize);
		}
		CommandLogInit();
	}

	internal override bool Connect(string ipport, string appID)
	{
		if (peerConnectionState != 0)
		{
			base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting. peerConnectionState: " + peerConnectionState);
			return false;
		}
		if ((int)debugOut >= 5)
		{
			base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
		}
		base.ServerAddress = ipport;
		InitPeerBase();
		if (appID == null)
		{
			appID = "LoadBalancing";
		}
		for (int i = 0; i < 32; i++)
		{
			INIT_BYTES[i + 9] = (byte)((i < appID.Length) ? ((byte)appID[i]) : 0);
		}
		base.Listener.DebugReturn(DebugLevel.ALL, "EnetPeer connect: Init data IPv6 bit set: " + INIT_BYTES[5]);
		initData = INIT_BYTES;
		rt = new SocketUdp(this);
		if (rt == null)
		{
			base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect().");
			return false;
		}
		if (rt.Connect())
		{
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsOutgoing.ControlCommandBytes += 44;
				TrafficStatsOutgoing.ControlCommandCount++;
			}
			peerConnectionState = ConnectionStateValue.Connecting;
			return true;
		}
		return false;
	}

	public override void OnConnect()
	{
		QueueOutgoingReliableCommand(new NCommand(this, 2, null, byte.MaxValue));
	}

	internal override void Disconnect()
	{
		if (peerConnectionState == ConnectionStateValue.Disconnected || peerConnectionState == ConnectionStateValue.Disconnecting)
		{
			return;
		}
		if (outgoingAcknowledgementsList != null)
		{
			lock (outgoingAcknowledgementsList)
			{
				outgoingAcknowledgementsList.Clear();
			}
		}
		if (sentReliableCommands != null)
		{
			lock (sentReliableCommands)
			{
				sentReliableCommands.Clear();
			}
		}
		lock (channels)
		{
			foreach (EnetChannel value in channels.Values)
			{
				value.clearAll();
			}
		}
		bool isSimulationEnabled = base.NetworkSimulationSettings.IsSimulationEnabled;
		base.NetworkSimulationSettings.IsSimulationEnabled = false;
		NCommand nCommand = new NCommand(this, 4, null, byte.MaxValue);
		QueueOutgoingReliableCommand(nCommand);
		SendOutgoingCommands();
		if (base.TrafficStatsEnabled)
		{
			TrafficStatsOutgoing.CountControlCommand(nCommand.Size);
		}
		base.NetworkSimulationSettings.IsSimulationEnabled = isSimulationEnabled;
		rt.Disconnect();
		peerConnectionState = ConnectionStateValue.Disconnected;
		base.Listener.OnStatusChanged(StatusCode.Disconnect);
	}

	internal override void StopConnection()
	{
		if (rt != null)
		{
			rt.Disconnect();
		}
		peerConnectionState = ConnectionStateValue.Disconnected;
		if (base.Listener != null)
		{
			base.Listener.OnStatusChanged(StatusCode.Disconnect);
		}
	}

	internal override void FetchServerTimestamp()
	{
		if (peerConnectionState != ConnectionStateValue.Connected)
		{
			if ((int)debugOut >= 3)
			{
				EnqueueDebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + peerConnectionState);
			}
		}
		else
		{
			CreateAndEnqueueCommand(12, new byte[0], byte.MaxValue);
		}
	}

	internal override bool DispatchIncomingCommands()
	{
		while (true)
		{
			MyAction myAction;
			lock (ActionQueue)
			{
				if (ActionQueue.Count <= 0)
				{
					break;
				}
				myAction = ActionQueue.Dequeue();
				goto IL_0042;
			}
			IL_0042:
			myAction();
		}
		NCommand value = null;
		lock (channels)
		{
			for (int i = 0; i < channelArray.Length; i++)
			{
				EnetChannel enetChannel = channelArray[i];
				if (enetChannel.incomingUnreliableCommandsList.Count > 0)
				{
					int num = int.MaxValue;
					foreach (int key in enetChannel.incomingUnreliableCommandsList.Keys)
					{
						NCommand nCommand = enetChannel.incomingUnreliableCommandsList[key];
						if (key < enetChannel.incomingUnreliableSequenceNumber || nCommand.reliableSequenceNumber < enetChannel.incomingReliableSequenceNumber)
						{
							commandsToRemove.Enqueue(key);
						}
						else if (limitOfUnreliableCommands > 0 && enetChannel.incomingUnreliableCommandsList.Count > limitOfUnreliableCommands)
						{
							commandsToRemove.Enqueue(key);
						}
						else if (key < num && nCommand.reliableSequenceNumber <= enetChannel.incomingReliableSequenceNumber)
						{
							num = key;
						}
					}
					while (commandsToRemove.Count > 0)
					{
						enetChannel.incomingUnreliableCommandsList.Remove(commandsToRemove.Dequeue());
					}
					if (num < int.MaxValue)
					{
						value = enetChannel.incomingUnreliableCommandsList[num];
					}
					if (value != null)
					{
						enetChannel.incomingUnreliableCommandsList.Remove(value.unreliableSequenceNumber);
						enetChannel.incomingUnreliableSequenceNumber = value.unreliableSequenceNumber;
						break;
					}
				}
				if (value != null || enetChannel.incomingReliableCommandsList.Count <= 0)
				{
					continue;
				}
				enetChannel.incomingReliableCommandsList.TryGetValue(enetChannel.incomingReliableSequenceNumber + 1, out value);
				if (value == null)
				{
					continue;
				}
				if (value.commandType != 8)
				{
					enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber;
					enetChannel.incomingReliableCommandsList.Remove(value.reliableSequenceNumber);
					break;
				}
				if (value.fragmentsRemaining > 0)
				{
					value = null;
					break;
				}
				byte[] array = new byte[value.totalLength];
				for (int j = value.startSequenceNumber; j < value.startSequenceNumber + value.fragmentCount; j++)
				{
					if (enetChannel.ContainsReliableSequenceNumber(j))
					{
						NCommand nCommand2 = enetChannel.FetchReliableSequenceNumber(j);
						Buffer.BlockCopy(nCommand2.Payload, 0, array, nCommand2.fragmentOffset, nCommand2.Payload.Length);
						enetChannel.incomingReliableCommandsList.Remove(nCommand2.reliableSequenceNumber);
						continue;
					}
					throw new Exception("command.fragmentsRemaining was 0, but not all fragments are found to be combined!");
				}
				if ((int)debugOut >= 5)
				{
					base.Listener.DebugReturn(DebugLevel.ALL, "assembled fragmented payload from " + value.fragmentCount + " parts. Dispatching now.");
				}
				value.Payload = array;
				value.Size = 12 * value.fragmentCount + value.totalLength;
				enetChannel.incomingReliableSequenceNumber = value.reliableSequenceNumber + value.fragmentCount - 1;
				break;
			}
		}
		if (value != null && value.Payload != null)
		{
			ByteCountCurrentDispatch = value.Size;
			CommandInCurrentDispatch = value;
			if (DeserializeMessageAndCallback(value.Payload))
			{
				CommandInCurrentDispatch = null;
				return true;
			}
			CommandInCurrentDispatch = null;
		}
		return false;
	}

	internal override bool SendAcksOnly()
	{
		if (peerConnectionState == ConnectionStateValue.Disconnected)
		{
			return false;
		}
		if (rt == null || !rt.Connected)
		{
			return false;
		}
		lock (udpBuffer)
		{
			int num = 0;
			udpBufferIndex = 12;
			if (crcEnabled)
			{
				udpBufferIndex += 4;
			}
			udpCommandCount = 0;
			timeInt = SupportClass.GetTickCount() - timeBase;
			lock (outgoingAcknowledgementsList)
			{
				if (outgoingAcknowledgementsList.Count > 0)
				{
					num = SerializeToBuffer(outgoingAcknowledgementsList);
					timeLastSendAck = timeInt;
				}
			}
			if (timeInt > timeoutInt && sentReliableCommands.Count > 0)
			{
				lock (sentReliableCommands)
				{
					foreach (NCommand sentReliableCommand in sentReliableCommands)
					{
						if (sentReliableCommand != null && sentReliableCommand.roundTripTimeout != 0 && timeInt - sentReliableCommand.commandSentTime > sentReliableCommand.roundTripTimeout)
						{
							sentReliableCommand.commandSentCount = 1;
							sentReliableCommand.roundTripTimeout = 0;
							sentReliableCommand.timeoutTime = int.MaxValue;
							sentReliableCommand.commandSentTime = timeInt;
						}
					}
				}
			}
			if (udpCommandCount <= 0)
			{
				return false;
			}
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsOutgoing.TotalPacketCount++;
				TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
			}
			SendData(udpBuffer, udpBufferIndex);
			return num > 0;
		}
	}

	internal override bool SendOutgoingCommands()
	{
		if (peerConnectionState == ConnectionStateValue.Disconnected)
		{
			return false;
		}
		if (!rt.Connected)
		{
			return false;
		}
		lock (udpBuffer)
		{
			int num = 0;
			udpBufferIndex = 12;
			if (crcEnabled)
			{
				udpBufferIndex += 4;
			}
			udpCommandCount = 0;
			timeInt = SupportClass.GetTickCount() - timeBase;
			timeLastSendOutgoing = timeInt;
			lock (outgoingAcknowledgementsList)
			{
				if (outgoingAcknowledgementsList.Count > 0)
				{
					num = SerializeToBuffer(outgoingAcknowledgementsList);
					timeLastSendAck = timeInt;
				}
			}
			if (!base.IsSendingOnlyAcks && timeInt > timeoutInt && sentReliableCommands.Count > 0)
			{
				lock (sentReliableCommands)
				{
					Queue<NCommand> queue = new Queue<NCommand>();
					foreach (NCommand sentReliableCommand in sentReliableCommands)
					{
						if (sentReliableCommand == null || timeInt - sentReliableCommand.commandSentTime <= sentReliableCommand.roundTripTimeout)
						{
							continue;
						}
						if (sentReliableCommand.commandSentCount > sentCountAllowance || timeInt > sentReliableCommand.timeoutTime)
						{
							if ((int)debugOut >= 2)
							{
								base.Listener.DebugReturn(DebugLevel.WARNING, string.Concat("Timeout-disconnect! Command: ", sentReliableCommand, " now: ", timeInt, " challenge: ", Convert.ToString(challenge, 16)));
							}
							if (CommandLog != null)
							{
								CommandLog.Enqueue(new CmdLogSentReliable(sentReliableCommand, timeInt, roundTripTime, roundTripTimeVariance, true));
								CommandLogResize();
							}
							peerConnectionState = ConnectionStateValue.Zombie;
							base.Listener.OnStatusChanged(StatusCode.TimeoutDisconnect);
							Disconnect();
							return false;
						}
						queue.Enqueue(sentReliableCommand);
					}
					while (queue.Count > 0)
					{
						NCommand nCommand = queue.Dequeue();
						QueueOutgoingReliableCommand(nCommand);
						sentReliableCommands.Remove(nCommand);
						reliableCommandsRepeated++;
						if ((int)debugOut >= 3)
						{
							base.Listener.DebugReturn(DebugLevel.INFO, string.Format("Resending: {0}. times out after: {1} sent: {3} now: {2} rtt/var: {4}/{5} last recv: {6}", nCommand, nCommand.roundTripTimeout, timeInt, nCommand.commandSentTime, roundTripTime, roundTripTimeVariance, SupportClass.GetTickCount() - timestampOfLastReceive));
						}
					}
				}
			}
			if (!base.IsSendingOnlyAcks && peerConnectionState == ConnectionStateValue.Connected && timePingInterval > 0 && sentReliableCommands.Count == 0 && timeInt - timeLastAckReceive > timePingInterval && !AreReliableCommandsInTransit() && udpBufferIndex + 12 < udpBuffer.Length)
			{
				NCommand nCommand2 = new NCommand(this, 5, null, byte.MaxValue);
				QueueOutgoingReliableCommand(nCommand2);
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.CountControlCommand(nCommand2.Size);
				}
			}
			if (!base.IsSendingOnlyAcks)
			{
				lock (channels)
				{
					for (int i = 0; i < channelArray.Length; i++)
					{
						EnetChannel enetChannel = channelArray[i];
						num += SerializeToBuffer(enetChannel.outgoingReliableCommandsList);
						num += SerializeToBuffer(enetChannel.outgoingUnreliableCommandsList);
					}
				}
			}
			if (udpCommandCount <= 0)
			{
				return false;
			}
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsOutgoing.TotalPacketCount++;
				TrafficStatsOutgoing.TotalCommandsInPackets += udpCommandCount;
			}
			SendData(udpBuffer, udpBufferIndex);
			return num > 0;
		}
	}

	private bool AreReliableCommandsInTransit()
	{
		lock (channels)
		{
			foreach (EnetChannel value in channels.Values)
			{
				if (value.outgoingReliableCommandsList.Count > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, bool sendReliable, byte channelId, bool encrypt, EgMessageType messageType)
	{
		if (peerConnectionState != ConnectionStateValue.Connected)
		{
			if ((int)debugOut >= 1)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + " Not connected. PeerState: " + peerConnectionState);
			}
			base.Listener.OnStatusChanged(StatusCode.SendError);
			return false;
		}
		if (channelId >= ChannelCount)
		{
			if ((int)debugOut >= 1)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + channelId + ")>= channelCount (" + ChannelCount + ").");
			}
			base.Listener.OnStatusChanged(StatusCode.SendError);
			return false;
		}
		byte[] payload = SerializeOperationToMessage(opCode, parameters, messageType, encrypt);
		return CreateAndEnqueueCommand((byte)(sendReliable ? 6 : 7), payload, channelId);
	}

	internal bool CreateAndEnqueueCommand(byte commandType, byte[] payload, byte channelNumber)
	{
		if (payload == null)
		{
			return false;
		}
		EnetChannel enetChannel = channels[channelNumber];
		ByteCountLastOperation = 0;
		int num = mtu - 12 - 36;
		if (payload.Length > num)
		{
			int fragmentCount = (payload.Length + num - 1) / num;
			int startSequenceNumber = enetChannel.outgoingReliableSequenceNumber + 1;
			int num2 = 0;
			for (int i = 0; i < payload.Length; i += num)
			{
				if (payload.Length - i < num)
				{
					num = payload.Length - i;
				}
				byte[] array = new byte[num];
				Buffer.BlockCopy(payload, i, array, 0, num);
				NCommand nCommand = new NCommand(this, 8, array, enetChannel.ChannelNumber);
				nCommand.fragmentNumber = num2;
				nCommand.startSequenceNumber = startSequenceNumber;
				nCommand.fragmentCount = fragmentCount;
				nCommand.totalLength = payload.Length;
				nCommand.fragmentOffset = i;
				QueueOutgoingReliableCommand(nCommand);
				ByteCountLastOperation += nCommand.Size;
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.CountFragmentOpCommand(nCommand.Size);
					TrafficStatsGameLevel.CountOperation(nCommand.Size);
				}
				num2++;
			}
		}
		else
		{
			NCommand nCommand2 = new NCommand(this, commandType, payload, enetChannel.ChannelNumber);
			if (nCommand2.commandFlags == 1)
			{
				QueueOutgoingReliableCommand(nCommand2);
				ByteCountLastOperation = nCommand2.Size;
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.CountReliableOpCommand(nCommand2.Size);
					TrafficStatsGameLevel.CountOperation(nCommand2.Size);
				}
			}
			else
			{
				QueueOutgoingUnreliableCommand(nCommand2);
				ByteCountLastOperation = nCommand2.Size;
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.CountUnreliableOpCommand(nCommand2.Size);
					TrafficStatsGameLevel.CountOperation(nCommand2.Size);
				}
			}
		}
		return true;
	}

	internal override byte[] SerializeOperationToMessage(byte opc, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
	{
		byte[] array;
		lock (SerializeMemStream)
		{
			SerializeMemStream.Position = 0L;
			SerializeMemStream.SetLength(0L);
			if (!encrypt)
			{
				SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
			}
			base.Protocol.SerializeOperationRequest(SerializeMemStream, opc, parameters, false);
			if (encrypt)
			{
				byte[] data = SerializeMemStream.ToArray();
				data = CryptoProvider.Encrypt(data);
				SerializeMemStream.Position = 0L;
				SerializeMemStream.SetLength(0L);
				SerializeMemStream.Write(messageHeader, 0, messageHeader.Length);
				SerializeMemStream.Write(data, 0, data.Length);
			}
			array = SerializeMemStream.ToArray();
		}
		if (messageType != EgMessageType.Operation)
		{
			array[messageHeader.Length - 1] = (byte)messageType;
		}
		if (encrypt)
		{
			array[messageHeader.Length - 1] = (byte)(array[messageHeader.Length - 1] | 0x80u);
		}
		return array;
	}

	internal int SerializeToBuffer(Queue<NCommand> commandList)
	{
		while (commandList.Count > 0)
		{
			NCommand nCommand = commandList.Peek();
			if (nCommand == null)
			{
				commandList.Dequeue();
				continue;
			}
			if (udpBufferIndex + nCommand.Size > udpBuffer.Length)
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + udpCommandCount + ". Commands left in queue: " + commandList.Count);
				}
				break;
			}
			Buffer.BlockCopy(nCommand.Serialize(base.Protocol), 0, udpBuffer, udpBufferIndex, nCommand.Size);
			udpBufferIndex += nCommand.Size;
			udpCommandCount++;
			if ((nCommand.commandFlags & 1) > 0)
			{
				QueueSentCommand(nCommand);
				if (CommandLog != null)
				{
					CommandLog.Enqueue(new CmdLogSentReliable(nCommand, timeInt, roundTripTime, roundTripTimeVariance));
					CommandLogResize();
				}
			}
			commandList.Dequeue();
		}
		return commandList.Count;
	}

	internal void SendData(byte[] data, int length)
	{
		try
		{
			int targetOffset = 0;
			base.Protocol.Serialize(peerID, data, ref targetOffset);
			data[2] = (byte)(crcEnabled ? 204 : 0);
			data[3] = udpCommandCount;
			targetOffset = 4;
			base.Protocol.Serialize(timeInt, data, ref targetOffset);
			base.Protocol.Serialize(challenge, data, ref targetOffset);
			if (crcEnabled)
			{
				base.Protocol.Serialize(0, data, ref targetOffset);
				uint value = SupportClass.CalculateCrc(data, length);
				targetOffset -= 4;
				base.Protocol.Serialize((int)value, data, ref targetOffset);
			}
			bytesOut += length;
			if (base.NetworkSimulationSettings.IsSimulationEnabled)
			{
				byte[] dataCopy = new byte[length];
				Buffer.BlockCopy(data, 0, dataCopy, 0, length);
				SendNetworkSimulated(delegate
				{
					rt.Send(dataCopy, length);
				});
			}
			else
			{
				rt.Send(data, length);
			}
		}
		catch (Exception ex)
		{
			if ((int)debugOut >= 1)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
			}
			SupportClass.WriteStackTrace(ex);
		}
	}

	internal void QueueSentCommand(NCommand command)
	{
		command.commandSentTime = timeInt;
		command.commandSentCount++;
		if (command.roundTripTimeout == 0)
		{
			command.roundTripTimeout = roundTripTime + 4 * roundTripTimeVariance;
			command.timeoutTime = timeInt + DisconnectTimeout;
		}
		else if (command.commandSentCount > base.QuickResendAttempts + 1)
		{
			command.roundTripTimeout *= 2;
		}
		lock (sentReliableCommands)
		{
			if (sentReliableCommands.Count == 0)
			{
				int num = command.commandSentTime + command.roundTripTimeout;
				if (num < timeoutInt)
				{
					timeoutInt = num;
				}
			}
			reliableCommandsSent++;
			sentReliableCommands.Add(command);
		}
		if (sentReliableCommands.Count >= warningSize && sentReliableCommands.Count % warningSize == 0)
		{
			base.Listener.OnStatusChanged(StatusCode.QueueSentWarning);
		}
	}

	internal void QueueOutgoingReliableCommand(NCommand command)
	{
		EnetChannel enetChannel = channels[command.commandChannelID];
		lock (enetChannel)
		{
			Queue<NCommand> outgoingReliableCommandsList = enetChannel.outgoingReliableCommandsList;
			if (outgoingReliableCommandsList.Count >= warningSize && outgoingReliableCommandsList.Count % warningSize == 0)
			{
				base.Listener.OnStatusChanged(StatusCode.QueueOutgoingReliableWarning);
			}
			if (command.reliableSequenceNumber == 0)
			{
				command.reliableSequenceNumber = ++enetChannel.outgoingReliableSequenceNumber;
			}
			outgoingReliableCommandsList.Enqueue(command);
		}
	}

	internal void QueueOutgoingUnreliableCommand(NCommand command)
	{
		Queue<NCommand> outgoingUnreliableCommandsList = channels[command.commandChannelID].outgoingUnreliableCommandsList;
		if (outgoingUnreliableCommandsList.Count >= warningSize && outgoingUnreliableCommandsList.Count % warningSize == 0)
		{
			base.Listener.OnStatusChanged(StatusCode.QueueOutgoingUnreliableWarning);
		}
		EnetChannel enetChannel = channels[command.commandChannelID];
		command.reliableSequenceNumber = enetChannel.outgoingReliableSequenceNumber;
		command.unreliableSequenceNumber = ++enetChannel.outgoingUnreliableSequenceNumber;
		outgoingUnreliableCommandsList.Enqueue(command);
	}

	internal void QueueOutgoingAcknowledgement(NCommand command)
	{
		lock (outgoingAcknowledgementsList)
		{
			if (outgoingAcknowledgementsList.Count >= warningSize && outgoingAcknowledgementsList.Count % warningSize == 0)
			{
				base.Listener.OnStatusChanged(StatusCode.QueueOutgoingAcksWarning);
			}
			outgoingAcknowledgementsList.Enqueue(command);
		}
	}

	internal override void ReceiveIncomingCommands(byte[] inBuff, int dataLength)
	{
		timestampOfLastReceive = SupportClass.GetTickCount();
		try
		{
			int offset = 0;
			short value;
			base.Protocol.Deserialize(out value, inBuff, ref offset);
			byte b = inBuff[offset++];
			byte b2 = inBuff[offset++];
			base.Protocol.Deserialize(out serverSentTime, inBuff, ref offset);
			int value2;
			base.Protocol.Deserialize(out value2, inBuff, ref offset);
			if (b == 204)
			{
				int value3;
				base.Protocol.Deserialize(out value3, inBuff, ref offset);
				bytesIn += 4L;
				offset -= 4;
				base.Protocol.Serialize(0, inBuff, ref offset);
				uint num = SupportClass.CalculateCrc(inBuff, dataLength);
				if (value3 != (int)num)
				{
					packetLossByCrc++;
					if (peerConnectionState != 0 && (int)debugOut >= 3)
					{
						EnqueueDebugReturn(DebugLevel.INFO, string.Format("Ignored package due to wrong CRC. Incoming:  {0:X} Local: {1:X}", (uint)value3, num));
					}
					return;
				}
			}
			bytesIn += 12L;
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.TotalPacketCount++;
				TrafficStatsIncoming.TotalCommandsInPackets += b2;
			}
			if (b2 > commandBufferSize || b2 <= 0)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, "too many/few incoming commands in package: " + b2 + " > " + commandBufferSize);
			}
			if (value2 != challenge)
			{
				packetLossByChallenge++;
				if (peerConnectionState != 0 && (int)debugOut >= 5)
				{
					EnqueueDebugReturn(DebugLevel.ALL, "Info: Ignoring received package due to wrong challenge. Challenge in-package!=local:" + value2 + "!=" + challenge + " Commands in it: " + b2);
				}
				return;
			}
			timeInt = SupportClass.GetTickCount() - timeBase;
			for (int i = 0; i < b2; i++)
			{
				NCommand readCommand = new NCommand(this, inBuff, ref offset);
				if (readCommand.commandType != 1)
				{
					EnqueueActionForDispatch(delegate
					{
						ExecuteCommand(readCommand);
					});
				}
				else
				{
					TrafficStatsIncoming.TimestampOfLastAck = SupportClass.GetTickCount();
					ExecuteCommand(readCommand);
				}
				if ((readCommand.commandFlags & 1) > 0)
				{
					if (InReliableLog != null)
					{
						InReliableLog.Enqueue(new CmdLogReceivedReliable(readCommand, timeInt, roundTripTime, roundTripTimeVariance, timeInt - timeLastSendOutgoing, timeInt - timeLastSendAck));
						CommandLogResize();
					}
					NCommand nCommand = NCommand.CreateAck(this, readCommand, serverSentTime);
					QueueOutgoingAcknowledgement(nCommand);
					if (base.TrafficStatsEnabled)
					{
						TrafficStatsIncoming.TimestampOfLastReliableCommand = SupportClass.GetTickCount();
						TrafficStatsOutgoing.CountControlCommand(nCommand.Size);
					}
				}
			}
		}
		catch (Exception ex)
		{
			if ((int)debugOut >= 1)
			{
				EnqueueDebugReturn(DebugLevel.ERROR, string.Format("Exception while reading commands from incoming data: {0}", ex));
			}
			SupportClass.WriteStackTrace(ex);
		}
	}

	internal bool ExecuteCommand(NCommand command)
	{
		bool flag = true;
		switch (command.commandType)
		{
		case 2:
		case 5:
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountControlCommand(command.Size);
			}
			break;
		case 4:
		{
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountControlCommand(command.Size);
			}
			StatusCode statusCode = StatusCode.DisconnectByServer;
			if (command.reservedByte == 1)
			{
				statusCode = StatusCode.DisconnectByServerLogic;
			}
			else if (command.reservedByte == 3)
			{
				statusCode = StatusCode.DisconnectByServerUserLimit;
			}
			if ((int)debugOut >= 3)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "Server " + base.ServerAddress + " sent disconnect. PeerId: " + (ushort)peerID + " RTT/Variance:" + roundTripTime + "/" + roundTripTimeVariance + " reason byte: " + command.reservedByte);
			}
			ConnectionStateValue connectionStateValue = peerConnectionState;
			peerConnectionState = ConnectionStateValue.Disconnecting;
			base.Listener.OnStatusChanged(statusCode);
			peerConnectionState = connectionStateValue;
			Disconnect();
			break;
		}
		case 1:
		{
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountControlCommand(command.Size);
			}
			timeLastAckReceive = timeInt;
			lastRoundTripTime = timeInt - command.ackReceivedSentTime;
			NCommand nCommand = RemoveSentReliableCommand(command.ackReceivedReliableSequenceNumber, command.commandChannelID);
			if (CommandLog != null)
			{
				CommandLog.Enqueue(new CmdLogReceivedAck(command, timeInt, roundTripTime, roundTripTimeVariance));
				CommandLogResize();
			}
			if (nCommand == null)
			{
				break;
			}
			if (nCommand.commandType == 12)
			{
				if (lastRoundTripTime <= roundTripTime)
				{
					serverTimeOffset = serverSentTime + (lastRoundTripTime >> 1) - SupportClass.GetTickCount();
					serverTimeOffsetIsAvailable = true;
				}
				else
				{
					FetchServerTimestamp();
				}
				break;
			}
			UpdateRoundTripTimeAndVariance(lastRoundTripTime);
			if (nCommand.commandType == 4 && peerConnectionState == ConnectionStateValue.Disconnecting)
			{
				if ((int)debugOut >= 3)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Received disconnect ACK by server");
				}
				EnqueueActionForDispatch(delegate
				{
					rt.Disconnect();
				});
			}
			else if (nCommand.commandType == 2)
			{
				roundTripTime = lastRoundTripTime;
			}
			break;
		}
		case 6:
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountReliableOpCommand(command.Size);
			}
			if (peerConnectionState == ConnectionStateValue.Connected)
			{
				flag = QueueIncomingCommand(command);
			}
			break;
		case 7:
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
			}
			if (peerConnectionState == ConnectionStateValue.Connected)
			{
				flag = QueueIncomingCommand(command);
			}
			break;
		case 8:
		{
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountFragmentOpCommand(command.Size);
			}
			if (peerConnectionState != ConnectionStateValue.Connected)
			{
				break;
			}
			if (command.fragmentNumber > command.fragmentCount || command.fragmentOffset >= command.totalLength || command.fragmentOffset + command.Payload.Length > command.totalLength)
			{
				if ((int)debugOut >= 1)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Received fragment has bad size: " + command);
				}
				break;
			}
			flag = QueueIncomingCommand(command);
			if (!flag)
			{
				break;
			}
			EnetChannel enetChannel = channels[command.commandChannelID];
			if (command.reliableSequenceNumber == command.startSequenceNumber)
			{
				command.fragmentsRemaining--;
				int num = command.startSequenceNumber + 1;
				while (command.fragmentsRemaining > 0 && num < command.startSequenceNumber + command.fragmentCount)
				{
					if (enetChannel.ContainsReliableSequenceNumber(num++))
					{
						command.fragmentsRemaining--;
					}
				}
			}
			else if (enetChannel.ContainsReliableSequenceNumber(command.startSequenceNumber))
			{
				NCommand nCommand2 = enetChannel.FetchReliableSequenceNumber(command.startSequenceNumber);
				nCommand2.fragmentsRemaining--;
			}
			break;
		}
		case 3:
			if (base.TrafficStatsEnabled)
			{
				TrafficStatsIncoming.CountControlCommand(command.Size);
			}
			if (peerConnectionState == ConnectionStateValue.Connecting)
			{
				command = new NCommand(this, 6, initData, 0);
				QueueOutgoingReliableCommand(command);
				if (base.TrafficStatsEnabled)
				{
					TrafficStatsOutgoing.CountControlCommand(command.Size);
				}
				peerConnectionState = ConnectionStateValue.Connected;
			}
			break;
		}
		return flag;
	}

	internal bool QueueIncomingCommand(NCommand command)
	{
		EnetChannel value;
		channels.TryGetValue(command.commandChannelID, out value);
		if (value == null)
		{
			if ((int)debugOut >= 1)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Received command for non-existing channel: " + command.commandChannelID);
			}
			return false;
		}
		if ((int)debugOut >= 5)
		{
			base.Listener.DebugReturn(DebugLevel.ALL, string.Concat("queueIncomingCommand() ", command, " channel seq# r/u: ", value.incomingReliableSequenceNumber, "/", value.incomingUnreliableSequenceNumber));
		}
		if (command.commandFlags == 1)
		{
			if (command.reliableSequenceNumber <= value.incomingReliableSequenceNumber)
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("incoming command ", command, " is old (not saving it). Dispatched incomingReliableSequenceNumber: ", value.incomingReliableSequenceNumber));
				}
				return false;
			}
			if (value.ContainsReliableSequenceNumber(command.reliableSequenceNumber))
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("Info: command was received before! Old/New: ", value.FetchReliableSequenceNumber(command.reliableSequenceNumber), "/", command, " inReliableSeq#: ", value.incomingReliableSequenceNumber));
				}
				return false;
			}
			if (value.incomingReliableCommandsList.Count >= warningSize && value.incomingReliableCommandsList.Count % warningSize == 0)
			{
				base.Listener.OnStatusChanged(StatusCode.QueueIncomingReliableWarning);
			}
			value.incomingReliableCommandsList.Add(command.reliableSequenceNumber, command);
			return true;
		}
		if (command.commandFlags == 0)
		{
			if (command.reliableSequenceNumber < value.incomingReliableSequenceNumber)
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "incoming reliable-seq# < Dispatched-rel-seq#. not saved.");
				}
				return true;
			}
			if (command.unreliableSequenceNumber <= value.incomingUnreliableSequenceNumber)
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "incoming unreliable-seq# < Dispatched-unrel-seq#. not saved.");
				}
				return true;
			}
			if (value.ContainsUnreliableSequenceNumber(command.unreliableSequenceNumber))
			{
				if ((int)debugOut >= 3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, string.Concat("command was received before! Old/New: ", value.incomingUnreliableCommandsList[command.unreliableSequenceNumber], "/", command));
				}
				return false;
			}
			if (value.incomingUnreliableCommandsList.Count >= warningSize && value.incomingUnreliableCommandsList.Count % warningSize == 0)
			{
				base.Listener.OnStatusChanged(StatusCode.QueueIncomingUnreliableWarning);
			}
			value.incomingUnreliableCommandsList.Add(command.unreliableSequenceNumber, command);
			return true;
		}
		return false;
	}

	internal NCommand RemoveSentReliableCommand(int ackReceivedReliableSequenceNumber, int ackReceivedChannel)
	{
		NCommand nCommand = null;
		lock (sentReliableCommands)
		{
			foreach (NCommand sentReliableCommand in sentReliableCommands)
			{
				if (sentReliableCommand != null && sentReliableCommand.reliableSequenceNumber == ackReceivedReliableSequenceNumber && sentReliableCommand.commandChannelID == ackReceivedChannel)
				{
					nCommand = sentReliableCommand;
					break;
				}
			}
			if (nCommand != null)
			{
				sentReliableCommands.Remove(nCommand);
				if (sentReliableCommands.Count > 0)
				{
					timeoutInt = timeInt + 25;
				}
			}
			else if ((int)debugOut >= 5 && peerConnectionState != ConnectionStateValue.Connected && peerConnectionState != ConnectionStateValue.Disconnecting)
			{
				EnqueueDebugReturn(DebugLevel.ALL, string.Format("No sent command for ACK (Ch: {0} Sq#: {1}). PeerState: {2}.", ackReceivedReliableSequenceNumber, ackReceivedChannel, peerConnectionState));
			}
		}
		return nCommand;
	}

	internal string CommandListToString(NCommand[] list)
	{
		if ((int)debugOut < 5)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < list.Length; i++)
		{
			stringBuilder.Append(i + "=");
			stringBuilder.Append(list[i]);
			stringBuilder.Append(" # ");
		}
		return stringBuilder.ToString();
	}
}
