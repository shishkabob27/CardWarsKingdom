// ExitGames.Client.Photon.NCommand
using System;
using ExitGames.Client;
using ExitGames.Client.Photon;

internal class NCommand : IComparable<NCommand>
{
	internal byte commandFlags;

	internal const int FLAG_RELIABLE = 1;

	internal const int FLAG_UNSEQUENCED = 2;

	internal const byte FV_UNRELIABLE = 0;

	internal const byte FV_RELIABLE = 1;

	internal const byte FV_UNRELIBALE_UNSEQUENCED = 2;

	internal byte commandType;

	internal const byte CT_NONE = 0;

	internal const byte CT_ACK = 1;

	internal const byte CT_CONNECT = 2;

	internal const byte CT_VERIFYCONNECT = 3;

	internal const byte CT_DISCONNECT = 4;

	internal const byte CT_PING = 5;

	internal const byte CT_SENDRELIABLE = 6;

	internal const byte CT_SENDUNRELIABLE = 7;

	internal const byte CT_SENDFRAGMENT = 8;

	internal const byte CT_EG_SERVERTIME = 12;

	internal byte commandChannelID;

	internal int reliableSequenceNumber;

	internal int unreliableSequenceNumber;

	internal int unsequencedGroupNumber;

	internal byte reservedByte = 4;

	internal int startSequenceNumber;

	internal int fragmentCount;

	internal int fragmentNumber;

	internal int totalLength;

	internal int fragmentOffset;

	internal int fragmentsRemaining;

	internal byte[] Payload;

	internal int commandSentTime;

	internal byte commandSentCount;

	internal int roundTripTimeout;

	internal int timeoutTime;

	internal int ackReceivedReliableSequenceNumber;

	internal int ackReceivedSentTime;

	internal const int HEADER_UDP_PACK_LENGTH = 12;

	internal const int CmdSizeMinimum = 12;

	internal const int CmdSizeAck = 20;

	internal const int CmdSizeConnect = 44;

	internal const int CmdSizeVerifyConnect = 44;

	internal const int CmdSizeDisconnect = 12;

	internal const int CmdSizePing = 12;

	internal const int CmdSizeReliableHeader = 12;

	internal const int CmdSizeUnreliableHeader = 16;

	internal const int CmdSizeFragmentHeader = 32;

	internal const int CmdSizeMaxHeader = 36;

	private byte[] completeCommand;

	internal int Size;

	internal NCommand(EnetPeer peer, byte commandType, byte[] payload, byte channel)
	{
		this.commandType = commandType;
		commandFlags = 1;
		commandChannelID = channel;
		Payload = payload;
		Size = 12;
		switch (this.commandType)
		{
		case 2:
		{
			Size = 44;
			Payload = new byte[32];
			Payload[0] = 0;
			Payload[1] = 0;
			int targetOffset = 2;
			peer.Protocol.Serialize((short)peer.mtu, Payload, ref targetOffset);
			Payload[4] = 0;
			Payload[5] = 0;
			Payload[6] = 128;
			Payload[7] = 0;
			Payload[11] = peer.ChannelCount;
			Payload[15] = 0;
			Payload[19] = 0;
			Payload[22] = 19;
			Payload[23] = 136;
			Payload[27] = 2;
			Payload[31] = 2;
			break;
		}
		case 4:
			Size = 12;
			if (peer.peerConnectionState != PeerBase.ConnectionStateValue.Connected)
			{
				commandFlags = 2;
				if (peer.peerConnectionState == PeerBase.ConnectionStateValue.Zombie)
				{
					reservedByte = 2;
				}
			}
			break;
		case 6:
			Size = 12 + payload.Length;
			break;
		case 7:
			Size = 16 + payload.Length;
			commandFlags = 0;
			break;
		case 8:
			Size = 32 + payload.Length;
			break;
		case 1:
			Size = 20;
			commandFlags = 0;
			break;
		case 3:
		case 5:
			break;
		}
	}

	internal static NCommand CreateAck(EnetPeer peer, NCommand commandToAck, int sentTime)
	{
		byte[] array = new byte[8];
		int targetOffset = 0;
		peer.Protocol.Serialize(commandToAck.reliableSequenceNumber, array, ref targetOffset);
		peer.Protocol.Serialize(sentTime, array, ref targetOffset);
		NCommand nCommand = new NCommand(peer, 1, array, commandToAck.commandChannelID);
		nCommand.ackReceivedReliableSequenceNumber = commandToAck.reliableSequenceNumber;
		nCommand.ackReceivedSentTime = sentTime;
		return nCommand;
	}

	internal NCommand(EnetPeer peer, byte[] inBuff, ref int readingOffset)
	{
		commandType = inBuff[readingOffset++];
		commandChannelID = inBuff[readingOffset++];
		commandFlags = inBuff[readingOffset++];
		reservedByte = inBuff[readingOffset++];
		peer.Protocol.Deserialize(out Size, inBuff, ref readingOffset);
		peer.Protocol.Deserialize(out reliableSequenceNumber, inBuff, ref readingOffset);
		peer.bytesIn += Size;
		switch (commandType)
		{
		case 1:
			peer.Protocol.Deserialize(out ackReceivedReliableSequenceNumber, inBuff, ref readingOffset);
			peer.Protocol.Deserialize(out ackReceivedSentTime, inBuff, ref readingOffset);
			break;
		case 6:
			Payload = new byte[Size - 12];
			break;
		case 7:
			peer.Protocol.Deserialize(out unreliableSequenceNumber, inBuff, ref readingOffset);
			Payload = new byte[Size - 16];
			break;
		case 8:
			peer.Protocol.Deserialize(out startSequenceNumber, inBuff, ref readingOffset);
			peer.Protocol.Deserialize(out fragmentCount, inBuff, ref readingOffset);
			peer.Protocol.Deserialize(out fragmentNumber, inBuff, ref readingOffset);
			peer.Protocol.Deserialize(out totalLength, inBuff, ref readingOffset);
			peer.Protocol.Deserialize(out fragmentOffset, inBuff, ref readingOffset);
			Payload = new byte[Size - 32];
			fragmentsRemaining = fragmentCount;
			break;
		case 3:
		{
			short value;
			peer.Protocol.Deserialize(out value, inBuff, ref readingOffset);
			readingOffset += 30;
			if (peer.peerID == -1)
			{
				peer.peerID = value;
			}
			break;
		}
		}
		if (Payload != null)
		{
			Buffer.BlockCopy(inBuff, readingOffset, Payload, 0, Payload.Length);
			readingOffset += Payload.Length;
		}
	}

	internal byte[] Serialize(IProtocol protocol)
	{
		if (completeCommand != null)
		{
			return completeCommand;
		}
		int num = ((Payload != null) ? Payload.Length : 0);
		int num2 = 12;
		if (commandType == 7)
		{
			num2 = 16;
		}
		else if (commandType == 8)
		{
			num2 = 32;
		}
		completeCommand = new byte[num2 + num];
		completeCommand[0] = commandType;
		completeCommand[1] = commandChannelID;
		completeCommand[2] = commandFlags;
		completeCommand[3] = reservedByte;
		int targetOffset = 4;
		protocol.Serialize(completeCommand.Length, completeCommand, ref targetOffset);
		protocol.Serialize(reliableSequenceNumber, completeCommand, ref targetOffset);
		if (commandType == 7)
		{
			targetOffset = 12;
			protocol.Serialize(unreliableSequenceNumber, completeCommand, ref targetOffset);
		}
		else if (commandType == 8)
		{
			targetOffset = 12;
			protocol.Serialize(startSequenceNumber, completeCommand, ref targetOffset);
			protocol.Serialize(fragmentCount, completeCommand, ref targetOffset);
			protocol.Serialize(fragmentNumber, completeCommand, ref targetOffset);
			protocol.Serialize(totalLength, completeCommand, ref targetOffset);
			protocol.Serialize(fragmentOffset, completeCommand, ref targetOffset);
		}
		if (num > 0)
		{
			Buffer.BlockCopy(Payload, 0, completeCommand, num2, num);
		}
		Payload = null;
		return completeCommand;
	}

	public int CompareTo(NCommand other)
	{
		if (((uint)commandFlags & (true ? 1u : 0u)) != 0)
		{
			return reliableSequenceNumber - other.reliableSequenceNumber;
		}
		return unreliableSequenceNumber - other.unreliableSequenceNumber;
	}

	public override string ToString()
	{
		if (commandType == 1)
		{
			return string.Format("CMD({1} ack for c#:{0} s#/time {2}/{3})", commandChannelID, commandType, ackReceivedReliableSequenceNumber, ackReceivedSentTime);
		}
		return string.Format("CMD({1} c#:{0} r/u: {2}/{3} st/r#/rt:{4}/{5}/{6})", commandChannelID, commandType, reliableSequenceNumber, unreliableSequenceNumber, commandSentTime, commandSentCount, timeoutTime);
	}
}
