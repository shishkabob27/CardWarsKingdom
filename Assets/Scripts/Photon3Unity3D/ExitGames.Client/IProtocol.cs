using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;

namespace ExitGames.Client
{
	public abstract class IProtocol
	{
		internal abstract string protocolType { get; }

		internal abstract byte[] VersionBytes { get; }

		public abstract void Serialize(MemoryStream dout, object serObject, bool setType);

		public abstract void Serialize(short value, byte[] target, ref int targetOffset);

		public abstract void Serialize(int value, byte[] target, ref int targetOffset);

		public abstract void Serialize(float value, byte[] target, ref int targetOffset);

		public abstract void SerializeShort(MemoryStream dout, short serObject, bool setType);

		public abstract void SerializeString(MemoryStream dout, string serObject, bool setType);

		public abstract void SerializeEventData(MemoryStream memStream, EventData serObject, bool setType);

		public abstract void SerializeOperationRequest(MemoryStream memStream, byte operationCode, Dictionary<byte, object> parameters, bool setType);

		public abstract void SerializeOperationResponse(MemoryStream memStream, OperationResponse serObject, bool setType);

		public abstract object Deserialize(MemoryStream din, byte type);

		public abstract void Deserialize(out short value, byte[] source, ref int offset);

		public abstract void Deserialize(out int value, byte[] source, ref int offset);

		public abstract void Deserialize(out float value, byte[] source, ref int offset);

		public abstract short DeserializeShort(MemoryStream din);

		public abstract byte DeserializeByte(MemoryStream din);

		public abstract EventData DeserializeEventData(MemoryStream din);

		public abstract OperationRequest DeserializeOperationRequest(MemoryStream din);

		public abstract OperationResponse DeserializeOperationResponse(MemoryStream memoryStream);

		public byte[] Serialize(object obj)
		{
			MemoryStream memoryStream = new MemoryStream(64);
			Serialize(memoryStream, obj, setType: true);
			return memoryStream.ToArray();
		}

		public object Deserialize(byte[] serializedData)
		{
			MemoryStream memoryStream = new MemoryStream(serializedData);
			return Deserialize(memoryStream, (byte)memoryStream.ReadByte());
		}

		public object DeserializeMessage(MemoryStream stream)
		{
			return Deserialize(stream, (byte)stream.ReadByte());
		}

		internal byte[] DeserializeRawMessage(MemoryStream stream)
		{
			return (byte[])Deserialize(stream, (byte)stream.ReadByte());
		}

		internal void SerializeMessage(MemoryStream ms, object msg)
		{
			Serialize(ms, msg, setType: true);
		}
	}
}
