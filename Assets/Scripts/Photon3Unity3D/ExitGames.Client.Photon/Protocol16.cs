// ExitGames.Client.Photon.Protocol16
#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExitGames.Client;
using ExitGames.Client.Photon;

public class Protocol16 : IProtocol
{
	public enum GpType : byte
	{
		Unknown = 0,
		Array = 121,
		Boolean = 111,
		Byte = 98,
		ByteArray = 120,
		ObjectArray = 122,
		Short = 107,
		Float = 102,
		Dictionary = 68,
		Double = 100,
		Hashtable = 104,
		Integer = 105,
		IntegerArray = 110,
		Long = 108,
		String = 115,
		StringArray = 97,
		Custom = 99,
		Null = 42,
		EventData = 101,
		OperationRequest = 113,
		OperationResponse = 112
	}

	private readonly byte[] versionBytes = new byte[2] { 1, 6 };

	private readonly byte[] memShort = new byte[2];

	private readonly long[] memLongBlock = new long[1];

	private readonly byte[] memLongBlockBytes = new byte[8];

	private readonly float[] memFloatBlock = new float[1];

	private readonly byte[] memFloatBlockBytes = new byte[4];

	private readonly double[] memDoubleBlock = new double[1];

	private readonly byte[] memDoubleBlockBytes = new byte[8];

	private readonly byte[] memInteger = new byte[4];

	private readonly byte[] memLong = new byte[8];

	private readonly byte[] memFloat = new byte[4];

	private readonly byte[] memDeserialize = new byte[4];

	private readonly byte[] memDouble = new byte[8];

	internal override string protocolType
	{
		get
		{
			return "GpBinaryV16";
		}
	}

	internal override byte[] VersionBytes
	{
		get
		{
			return versionBytes;
		}
	}

	private bool SerializeCustom(MemoryStream dout, object serObject)
	{
		CustomType value;
		if (Protocol.TypeDict.TryGetValue(serObject.GetType(), out value))
		{
			if (value.SerializeStreamFunction == null)
			{
				byte[] array = value.SerializeFunction(serObject);
				dout.WriteByte(99);
				dout.WriteByte(value.Code);
				SerializeShort(dout, (short)array.Length, false);
				dout.Write(array, 0, array.Length);
				return true;
			}
			dout.WriteByte(99);
			dout.WriteByte(value.Code);
			long position = dout.Position;
			dout.Position += 2L;
			short num = value.SerializeStreamFunction(dout, serObject);
			long position2 = dout.Position;
			dout.Position = position;
			SerializeShort(dout, num, false);
			dout.Position += num;
			if (dout.Position != position2)
			{
				throw new Exception("Serialization failed. Stream position corrupted. Should be " + position2 + " is now: " + dout.Position + " serializedLength: " + num);
			}
			return true;
		}
		return false;
	}

	private object DeserializeCustom(MemoryStream din, byte customTypeCode)
	{
		short num = DeserializeShort(din);
		CustomType value;
		if (Protocol.CodeDict.TryGetValue(customTypeCode, out value))
		{
			if (value.DeserializeStreamFunction == null)
			{
				byte[] array = new byte[num];
				din.Read(array, 0, num);
				return value.DeserializeFunction(array);
			}
			long position = din.Position;
			object result = value.DeserializeStreamFunction(din, num);
			int num2 = (int)(din.Position - position);
			if (num2 != num)
			{
				din.Position = position + num;
			}
			return result;
		}
		return null;
	}

	private Type GetTypeOfCode(byte typeCode)
	{
		switch (typeCode)
		{
		case 105:
			return typeof(int);
		case 115:
			return typeof(string);
		case 97:
			return typeof(string[]);
		case 120:
			return typeof(byte[]);
		case 110:
			return typeof(int[]);
		case 104:
			return typeof(ExitGames.Client.Photon.Hashtable);
		case 68:
			return typeof(IDictionary);
		case 111:
			return typeof(bool);
		case 107:
			return typeof(short);
		case 108:
			return typeof(long);
		case 98:
			return typeof(byte);
		case 102:
			return typeof(float);
		case 100:
			return typeof(double);
		case 121:
			return typeof(Array);
		case 99:
			return typeof(CustomType);
		case 122:
			return typeof(object[]);
		case 101:
			return typeof(EventData);
		case 113:
			return typeof(OperationRequest);
		case 112:
			return typeof(OperationResponse);
		case 0:
		case 42:
			return typeof(object);
		default:
			UnityEngine.Debug.LogError("missing type: " + typeCode);
			throw new Exception("deserialize(): " + typeCode);
		}
	}

	private GpType GetCodeOfType(Type type)
	{
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.Byte:
			return GpType.Byte;
		case TypeCode.String:
			return GpType.String;
		case TypeCode.Boolean:
			return GpType.Boolean;
		case TypeCode.Int16:
			return GpType.Short;
		case TypeCode.Int32:
			return GpType.Integer;
		case TypeCode.Int64:
			return GpType.Long;
		case TypeCode.Single:
			return GpType.Float;
		case TypeCode.Double:
			return GpType.Double;
		default:
			if (type.IsArray)
			{
				if (type == typeof(byte[]))
				{
					return GpType.ByteArray;
				}
				return GpType.Array;
			}
			if (type == typeof(ExitGames.Client.Photon.Hashtable))
			{
				return GpType.Hashtable;
			}
			if (type.IsGenericType && typeof(Dictionary<, >) == type.GetGenericTypeDefinition())
			{
				return GpType.Dictionary;
			}
			if (type == typeof(EventData))
			{
				return GpType.EventData;
			}
			if (type == typeof(OperationRequest))
			{
				return GpType.OperationRequest;
			}
			if (type == typeof(OperationResponse))
			{
				return GpType.OperationResponse;
			}
			return GpType.Unknown;
		}
	}

	private Array CreateArrayByType(byte arrayType, short length)
	{
		return Array.CreateInstance(GetTypeOfCode(arrayType), length);
	}

	private void SerializeOperationRequest(MemoryStream memStream, OperationRequest serObject, bool setType)
	{
		SerializeOperationRequest(memStream, serObject.OperationCode, serObject.Parameters, setType);
	}

	public override void SerializeOperationRequest(MemoryStream memStream, byte operationCode, Dictionary<byte, object> parameters, bool setType)
	{
		if (setType)
		{
			memStream.WriteByte(113);
		}
		memStream.WriteByte(operationCode);
		SerializeParameterTable(memStream, parameters);
	}

	public override OperationRequest DeserializeOperationRequest(MemoryStream din)
	{
		OperationRequest operationRequest = new OperationRequest();
		operationRequest.OperationCode = DeserializeByte(din);
		operationRequest.Parameters = DeserializeParameterTable(din);
		return operationRequest;
	}

	public override void SerializeOperationResponse(MemoryStream memStream, OperationResponse serObject, bool setType)
	{
		if (setType)
		{
			memStream.WriteByte(112);
		}
		memStream.WriteByte(serObject.OperationCode);
		SerializeShort(memStream, serObject.ReturnCode, false);
		if (string.IsNullOrEmpty(serObject.DebugMessage))
		{
			memStream.WriteByte(42);
		}
		else
		{
			SerializeString(memStream, serObject.DebugMessage, false);
		}
		SerializeParameterTable(memStream, serObject.Parameters);
	}

	public override OperationResponse DeserializeOperationResponse(MemoryStream memoryStream)
	{
		OperationResponse operationResponse = new OperationResponse();
		operationResponse.OperationCode = DeserializeByte(memoryStream);
		operationResponse.ReturnCode = DeserializeShort(memoryStream);
		operationResponse.DebugMessage = Deserialize(memoryStream, DeserializeByte(memoryStream)) as string;
		operationResponse.Parameters = DeserializeParameterTable(memoryStream);
		return operationResponse;
	}

	public override void SerializeEventData(MemoryStream memStream, EventData serObject, bool setType)
	{
		if (setType)
		{
			memStream.WriteByte(101);
		}
		memStream.WriteByte(serObject.Code);
		SerializeParameterTable(memStream, serObject.Parameters);
	}

	public override EventData DeserializeEventData(MemoryStream din)
	{
		EventData eventData = new EventData();
		eventData.Code = DeserializeByte(din);
		eventData.Parameters = DeserializeParameterTable(din);
		return eventData;
	}

	private void SerializeParameterTable(MemoryStream memStream, Dictionary<byte, object> parameters)
	{
		if (parameters == null || parameters.Count == 0)
		{
			SerializeShort(memStream, 0, false);
			return;
		}
		SerializeShort(memStream, (short)parameters.Count, false);
		foreach (KeyValuePair<byte, object> parameter in parameters)
		{
			memStream.WriteByte(parameter.Key);
			Serialize(memStream, parameter.Value, true);
		}
	}

	private Dictionary<byte, object> DeserializeParameterTable(MemoryStream memoryStream)
	{
		short num = DeserializeShort(memoryStream);
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>(num);
		for (int i = 0; i < num; i++)
		{
			byte key = (byte)memoryStream.ReadByte();
			object obj2 = (dictionary[key] = Deserialize(memoryStream, (byte)memoryStream.ReadByte()));
		}
		return dictionary;
	}

	public override void Serialize(MemoryStream dout, object serObject, bool setType)
	{
		if (serObject == null)
		{
			if (setType)
			{
				dout.WriteByte(42);
			}
			return;
		}
		switch (GetCodeOfType(serObject.GetType()))
		{
		case GpType.Byte:
			SerializeByte(dout, (byte)serObject, setType);
			break;
		case GpType.String:
			SerializeString(dout, (string)serObject, setType);
			break;
		case GpType.Boolean:
			SerializeBoolean(dout, (bool)serObject, setType);
			break;
		case GpType.Short:
			SerializeShort(dout, (short)serObject, setType);
			break;
		case GpType.Integer:
			SerializeInteger(dout, (int)serObject, setType);
			break;
		case GpType.Long:
			SerializeLong(dout, (long)serObject, setType);
			break;
		case GpType.Float:
			SerializeFloat(dout, (float)serObject, setType);
			break;
		case GpType.Double:
			SerializeDouble(dout, (double)serObject, setType);
			break;
		case GpType.Hashtable:
			SerializeHashTable(dout, (ExitGames.Client.Photon.Hashtable)serObject, setType);
			break;
		case GpType.ByteArray:
			SerializeByteArray(dout, (byte[])serObject, setType);
			break;
		case GpType.Array:
			if (serObject is int[])
			{
				SerializeIntArrayOptimized(dout, (int[])serObject, setType);
			}
			else if (serObject.GetType().GetElementType() == typeof(object))
			{
				SerializeObjectArray(dout, serObject as object[], setType);
			}
			else
			{
				SerializeArray(dout, (Array)serObject, setType);
			}
			break;
		case GpType.Dictionary:
			SerializeDictionary(dout, (IDictionary)serObject, setType);
			break;
		case GpType.EventData:
			SerializeEventData(dout, (EventData)serObject, setType);
			break;
		case GpType.OperationResponse:
			SerializeOperationResponse(dout, (OperationResponse)serObject, setType);
			break;
		case GpType.OperationRequest:
			SerializeOperationRequest(dout, (OperationRequest)serObject, setType);
			break;
		default:
			if (!SerializeCustom(dout, serObject))
			{
				throw new Exception("cannot serialize(): " + serObject.GetType());
			}
			break;
		}
	}

	private void SerializeByte(MemoryStream dout, byte serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(98);
		}
		dout.WriteByte(serObject);
	}

	private void SerializeBoolean(MemoryStream dout, bool serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(111);
		}
		dout.WriteByte((byte)(serObject ? 1 : 0));
	}

	public override void SerializeShort(MemoryStream dout, short serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(107);
		}
		lock (memShort)
		{
			byte[] array = memShort;
			array[0] = (byte)(serObject >> 8);
			array[1] = (byte)serObject;
			dout.Write(array, 0, 2);
		}
	}

	public override void Serialize(short value, byte[] target, ref int targetOffset)
	{
		target[targetOffset++] = (byte)(value >> 8);
		target[targetOffset++] = (byte)value;
	}

	private void SerializeInteger(MemoryStream dout, int serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(105);
		}
		lock (memInteger)
		{
			byte[] array = memInteger;
			array[0] = (byte)(serObject >> 24);
			array[1] = (byte)(serObject >> 16);
			array[2] = (byte)(serObject >> 8);
			array[3] = (byte)serObject;
			dout.Write(array, 0, 4);
		}
	}

	public override void Serialize(int value, byte[] target, ref int targetOffset)
	{
		target[targetOffset++] = (byte)(value >> 24);
		target[targetOffset++] = (byte)(value >> 16);
		target[targetOffset++] = (byte)(value >> 8);
		target[targetOffset++] = (byte)value;
	}

	private void SerializeLong(MemoryStream dout, long serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(108);
		}
		lock (memLongBlock)
		{
			memLongBlock[0] = serObject;
			Buffer.BlockCopy(memLongBlock, 0, memLongBlockBytes, 0, 8);
			byte[] array = memLongBlockBytes;
			if (BitConverter.IsLittleEndian)
			{
				byte b = array[0];
				byte b2 = array[1];
				byte b3 = array[2];
				byte b4 = array[3];
				array[0] = array[7];
				array[1] = array[6];
				array[2] = array[5];
				array[3] = array[4];
				array[4] = b4;
				array[5] = b3;
				array[6] = b2;
				array[7] = b;
			}
			dout.Write(array, 0, 8);
		}
	}

	private void SerializeFloat(MemoryStream dout, float serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(102);
		}
		lock (memFloatBlockBytes)
		{
			memFloatBlock[0] = serObject;
			Buffer.BlockCopy(memFloatBlock, 0, memFloatBlockBytes, 0, 4);
			if (BitConverter.IsLittleEndian)
			{
				byte b = memFloatBlockBytes[0];
				byte b2 = memFloatBlockBytes[1];
				memFloatBlockBytes[0] = memFloatBlockBytes[3];
				memFloatBlockBytes[1] = memFloatBlockBytes[2];
				memFloatBlockBytes[2] = b2;
				memFloatBlockBytes[3] = b;
			}
			dout.Write(memFloatBlockBytes, 0, 4);
		}
	}

	public override void Serialize(float value, byte[] target, ref int targetOffset)
	{
		lock (memFloatBlock)
		{
			memFloatBlock[0] = value;
			Buffer.BlockCopy(memFloatBlock, 0, target, targetOffset, 4);
		}
		if (BitConverter.IsLittleEndian)
		{
			byte b = target[targetOffset];
			byte b2 = target[targetOffset + 1];
			target[targetOffset] = target[targetOffset + 3];
			target[targetOffset + 1] = target[targetOffset + 2];
			target[targetOffset + 2] = b2;
			target[targetOffset + 3] = b;
		}
		targetOffset += 4;
	}

	private void SerializeDouble(MemoryStream dout, double serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(100);
		}
		lock (memDoubleBlockBytes)
		{
			memDoubleBlock[0] = serObject;
			Buffer.BlockCopy(memDoubleBlock, 0, memDoubleBlockBytes, 0, 8);
			byte[] array = memDoubleBlockBytes;
			if (BitConverter.IsLittleEndian)
			{
				byte b = array[0];
				byte b2 = array[1];
				byte b3 = array[2];
				byte b4 = array[3];
				array[0] = array[7];
				array[1] = array[6];
				array[2] = array[5];
				array[3] = array[4];
				array[4] = b4;
				array[5] = b3;
				array[6] = b2;
				array[7] = b;
			}
			dout.Write(array, 0, 8);
		}
	}

	public override void SerializeString(MemoryStream dout, string serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(115);
		}
		byte[] bytes = Encoding.UTF8.GetBytes(serObject);
		if (bytes.Length > 32767)
		{
			throw new NotSupportedException("Strings that exceed a UTF8-encoded byte-length of 32767 (short.MaxValue) are not supported. Yours is: " + bytes.Length);
		}
		SerializeShort(dout, (short)bytes.Length, false);
		dout.Write(bytes, 0, bytes.Length);
	}

	private void SerializeArray(MemoryStream dout, Array serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(121);
		}
		if (serObject.Length > 32767)
		{
			throw new NotSupportedException("String[] that exceed 32767 (short.MaxValue) entries are not supported. Yours is: " + serObject.Length);
		}
		SerializeShort(dout, (short)serObject.Length, false);
		Type elementType = serObject.GetType().GetElementType();
		GpType codeOfType = GetCodeOfType(elementType);
		if (codeOfType != 0)
		{
			dout.WriteByte((byte)codeOfType);
			if (codeOfType == GpType.Dictionary)
			{
				bool setKeyType;
				bool setValueType;
				SerializeDictionaryHeader(dout, serObject, out setKeyType, out setValueType);
				for (int i = 0; i < serObject.Length; i++)
				{
					object value = serObject.GetValue(i);
					SerializeDictionaryElements(dout, value, setKeyType, setValueType);
				}
			}
			else
			{
				for (int j = 0; j < serObject.Length; j++)
				{
					object value2 = serObject.GetValue(j);
					Serialize(dout, value2, false);
				}
			}
			return;
		}
		CustomType value3;
		if (Protocol.TypeDict.TryGetValue(elementType, out value3))
		{
			dout.WriteByte(99);
			dout.WriteByte(value3.Code);
			for (int k = 0; k < serObject.Length; k++)
			{
				object value4 = serObject.GetValue(k);
				if (value3.SerializeStreamFunction == null)
				{
					byte[] array = value3.SerializeFunction(value4);
					SerializeShort(dout, (short)array.Length, false);
					dout.Write(array, 0, array.Length);
					continue;
				}
				long position = dout.Position;
				dout.Position += 2L;
				short num = value3.SerializeStreamFunction(dout, value4);
				long position2 = dout.Position;
				dout.Position = position;
				SerializeShort(dout, num, false);
				dout.Position += num;
				if (dout.Position != position2)
				{
					throw new Exception("Serialization failed. Stream position corrupted. Should be " + position2 + " is now: " + dout.Position + " serializedLength: " + num);
				}
			}
			return;
		}
		throw new NotSupportedException("cannot serialize array of type " + elementType);
	}

	private void SerializeByteArray(MemoryStream dout, byte[] serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(120);
		}
		SerializeInteger(dout, serObject.Length, false);
		dout.Write(serObject, 0, serObject.Length);
	}

	private void SerializeIntArrayOptimized(MemoryStream inWriter, int[] serObject, bool setType)
	{
		if (setType)
		{
			inWriter.WriteByte(121);
		}
		SerializeShort(inWriter, (short)serObject.Length, false);
		inWriter.WriteByte(105);
		byte[] array = new byte[serObject.Length * 4];
		int num = 0;
		for (int i = 0; i < serObject.Length; i++)
		{
			array[num++] = (byte)(serObject[i] >> 24);
			array[num++] = (byte)(serObject[i] >> 16);
			array[num++] = (byte)(serObject[i] >> 8);
			array[num++] = (byte)serObject[i];
		}
		inWriter.Write(array, 0, array.Length);
	}

	private void SerializeStringArray(MemoryStream dout, string[] serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(97);
		}
		SerializeShort(dout, (short)serObject.Length, false);
		for (int i = 0; i < serObject.Length; i++)
		{
			SerializeString(dout, serObject[i], false);
		}
	}

	private void SerializeObjectArray(MemoryStream dout, object[] objects, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(122);
		}
		SerializeShort(dout, (short)objects.Length, false);
		foreach (object serObject in objects)
		{
			Serialize(dout, serObject, true);
		}
	}

	private void SerializeHashTable(MemoryStream dout, ExitGames.Client.Photon.Hashtable serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(104);
		}
		SerializeShort(dout, (short)serObject.Count, false);
		Dictionary<object, object>.KeyCollection keys = serObject.Keys;
		foreach (object item in keys)
		{
			Serialize(dout, item, true);
			Serialize(dout, serObject[item], true);
		}
	}

	private void SerializeDictionary(MemoryStream dout, IDictionary serObject, bool setType)
	{
		if (setType)
		{
			dout.WriteByte(68);
		}
		bool setKeyType;
		bool setValueType;
		SerializeDictionaryHeader(dout, serObject, out setKeyType, out setValueType);
		SerializeDictionaryElements(dout, serObject, setKeyType, setValueType);
	}

	private void SerializeDictionaryHeader(MemoryStream writer, Type dictType)
	{
		bool setKeyType;
		bool setValueType;
		SerializeDictionaryHeader(writer, dictType, out setKeyType, out setValueType);
	}

	private void SerializeDictionaryHeader(MemoryStream writer, object dict, out bool setKeyType, out bool setValueType)
	{
		Type[] genericArguments = dict.GetType().GetGenericArguments();
		setKeyType = genericArguments[0] == typeof(object);
		setValueType = genericArguments[1] == typeof(object);
		if (setKeyType)
		{
			writer.WriteByte(0);
		}
		else
		{
			GpType codeOfType = GetCodeOfType(genericArguments[0]);
			if (codeOfType == GpType.Unknown || codeOfType == GpType.Dictionary)
			{
				throw new Exception("Unexpected - cannot serialize Dictionary with key type: " + genericArguments[0]);
			}
			writer.WriteByte((byte)codeOfType);
		}
		if (setValueType)
		{
			writer.WriteByte(0);
			return;
		}
		GpType codeOfType2 = GetCodeOfType(genericArguments[1]);
		if (codeOfType2 == GpType.Unknown)
		{
			throw new Exception("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[0]);
		}
		writer.WriteByte((byte)codeOfType2);
		if (codeOfType2 == GpType.Dictionary)
		{
			SerializeDictionaryHeader(writer, genericArguments[1]);
		}
	}

	private void SerializeDictionaryElements(MemoryStream writer, object dict, bool setKeyType, bool setValueType)
	{
		IDictionary dictionary = (IDictionary)dict;
		SerializeShort(writer, (short)dictionary.Count, false);
		foreach (DictionaryEntry item in dictionary)
		{
			if (!setValueType && item.Value == null)
			{
				throw new Exception("Can't serialize null in Dictionary with specific value-type.");
			}
			if (!setKeyType && item.Key == null)
			{
				throw new Exception("Can't serialize null in Dictionary with specific key-type.");
			}
			Serialize(writer, item.Key, setKeyType);
			Serialize(writer, item.Value, setValueType);
		}
	}

	public override object Deserialize(MemoryStream din, byte type)
	{
		switch (type)
		{
		case 105:
			return DeserializeInteger(din);
		case 115:
			return DeserializeString(din);
		case 97:
			return DeserializeStringArray(din);
		case 120:
			return DeserializeByteArray(din);
		case 110:
			return DeserializeIntArray(din);
		case 104:
			return DeserializeHashTable(din);
		case 68:
			return DeserializeDictionary(din);
		case 111:
			return DeserializeBoolean(din);
		case 107:
			return DeserializeShort(din);
		case 108:
			return DeserializeLong(din);
		case 98:
			return DeserializeByte(din);
		case 102:
			return DeserializeFloat(din);
		case 100:
			return DeserializeDouble(din);
		case 121:
			return DeserializeArray(din);
		case 99:
		{
			byte customTypeCode = (byte)din.ReadByte();
			return DeserializeCustom(din, customTypeCode);
		}
		case 122:
			return DeserializeObjectArray(din);
		case 101:
			return DeserializeEventData(din);
		case 113:
			return DeserializeOperationRequest(din);
		case 112:
			return DeserializeOperationResponse(din);
		case 0:
		case 42:
			return null;
		default:
			UnityEngine.Debug.Log("missing type: " + type);
			throw new Exception("deserialize(): " + type);
		}
	}

	public override byte DeserializeByte(MemoryStream din)
	{
		return (byte)din.ReadByte();
	}

	private bool DeserializeBoolean(MemoryStream din)
	{
		return din.ReadByte() != 0;
	}

	public override short DeserializeShort(MemoryStream din)
	{
		lock (memShort)
		{
			byte[] array = memShort;
			din.Read(array, 0, 2);
			return (short)((array[0] << 8) | array[1]);
		}
	}

	public override void Deserialize(out short value, byte[] source, ref int offset)
	{
		value = (short)((source[offset++] << 8) | source[offset++]);
	}

	private int DeserializeInteger(MemoryStream din)
	{
		lock (memInteger)
		{
			byte[] array = memInteger;
			din.Read(array, 0, 4);
			return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
		}
	}

	public override void Deserialize(out int value, byte[] source, ref int offset)
	{
		value = (source[offset++] << 24) | (source[offset++] << 16) | (source[offset++] << 8) | source[offset++];
	}

	private long DeserializeLong(MemoryStream din)
	{
		lock (memLong)
		{
			byte[] array = memLong;
			din.Read(array, 0, 8);
			if (BitConverter.IsLittleEndian)
			{
				return (long)(((ulong)array[0] << 56) | ((ulong)array[1] << 48) | ((ulong)array[2] << 40) | ((ulong)array[3] << 32) | ((ulong)array[4] << 24) | ((ulong)array[5] << 16) | ((ulong)array[6] << 8) | array[7]);
			}
			return BitConverter.ToInt64(array, 0);
		}
	}

	private float DeserializeFloat(MemoryStream din)
	{
		lock (memFloat)
		{
			byte[] array = memFloat;
			din.Read(array, 0, 4);
			if (BitConverter.IsLittleEndian)
			{
				byte b = array[0];
				byte b2 = array[1];
				array[0] = array[3];
				array[1] = array[2];
				array[2] = b2;
				array[3] = b;
			}
			return BitConverter.ToSingle(array, 0);
		}
	}

	public override void Deserialize(out float value, byte[] source, ref int offset)
	{
		if (BitConverter.IsLittleEndian)
		{
			lock (memDeserialize)
			{
				byte[] array = memDeserialize;
				array[3] = source[offset++];
				array[2] = source[offset++];
				array[1] = source[offset++];
				array[0] = source[offset++];
				value = BitConverter.ToSingle(array, 0);
				return;
			}
		}
		value = BitConverter.ToSingle(source, offset);
		offset += 4;
	}

	private double DeserializeDouble(MemoryStream din)
	{
		lock (memDouble)
		{
			byte[] array = memDouble;
			din.Read(array, 0, 8);
			if (BitConverter.IsLittleEndian)
			{
				byte b = array[0];
				byte b2 = array[1];
				byte b3 = array[2];
				byte b4 = array[3];
				array[0] = array[7];
				array[1] = array[6];
				array[2] = array[5];
				array[3] = array[4];
				array[4] = b4;
				array[5] = b3;
				array[6] = b2;
				array[7] = b;
			}
			return BitConverter.ToDouble(array, 0);
		}
	}

	private string DeserializeString(MemoryStream din)
	{
		short num = DeserializeShort(din);
		if (num == 0)
		{
			return "";
		}
		byte[] array = new byte[num];
		din.Read(array, 0, array.Length);
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	private Array DeserializeArray(MemoryStream din)
	{
		short num = DeserializeShort(din);
		byte b = (byte)din.ReadByte();
		Array array;
		switch (b)
		{
		case 121:
		{
			Array array2 = DeserializeArray(din);
			Type type = array2.GetType();
			array = Array.CreateInstance(type, num);
			array.SetValue(array2, 0);
			for (short num3 = 1; num3 < num; num3 = (short)(num3 + 1))
			{
				array2 = DeserializeArray(din);
				array.SetValue(array2, num3);
			}
			break;
		}
		case 120:
		{
			array = Array.CreateInstance(typeof(byte[]), num);
			for (short num5 = 0; num5 < num; num5 = (short)(num5 + 1))
			{
				Array value2 = DeserializeByteArray(din);
				array.SetValue(value2, num5);
			}
			break;
		}
		case 99:
		{
			byte b2 = (byte)din.ReadByte();
			CustomType value;
			if (Protocol.CodeDict.TryGetValue(b2, out value))
			{
				array = Array.CreateInstance(value.Type, num);
				for (int i = 0; i < num; i++)
				{
					short num4 = DeserializeShort(din);
					if (value.DeserializeStreamFunction == null)
					{
						byte[] array3 = new byte[num4];
						din.Read(array3, 0, num4);
						array.SetValue(value.DeserializeFunction(array3), i);
					}
					else
					{
						array.SetValue(value.DeserializeStreamFunction(din, num4), i);
					}
				}
				break;
			}
			throw new Exception("Cannot find deserializer for custom type: " + b2);
		}
		case 68:
		{
			Array arrayResult = null;
			DeserializeDictionaryArray(din, num, out arrayResult);
			return arrayResult;
		}
		default:
		{
			array = CreateArrayByType(b, num);
			for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
			{
				array.SetValue(Deserialize(din, b), num2);
			}
			break;
		}
		}
		return array;
	}

	private byte[] DeserializeByteArray(MemoryStream din)
	{
		int num = DeserializeInteger(din);
		byte[] array = new byte[num];
		din.Read(array, 0, num);
		return array;
	}

	private int[] DeserializeIntArray(MemoryStream din)
	{
		int num = DeserializeInteger(din);
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = DeserializeInteger(din);
		}
		return array;
	}

	private string[] DeserializeStringArray(MemoryStream din)
	{
		int num = DeserializeShort(din);
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = DeserializeString(din);
		}
		return array;
	}

	private object[] DeserializeObjectArray(MemoryStream din)
	{
		short num = DeserializeShort(din);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			byte type = (byte)din.ReadByte();
			array[i] = Deserialize(din, type);
		}
		return array;
	}

	private ExitGames.Client.Photon.Hashtable DeserializeHashTable(MemoryStream din)
	{
		int num = DeserializeShort(din);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable(num);
		for (int i = 0; i < num; i++)
		{
			object key = Deserialize(din, (byte)din.ReadByte());
			object obj2 = (hashtable[key] = Deserialize(din, (byte)din.ReadByte()));
		}
		return hashtable;
	}

	private IDictionary DeserializeDictionary(MemoryStream din)
	{
		byte b = (byte)din.ReadByte();
		byte b2 = (byte)din.ReadByte();
		int num = DeserializeShort(din);
		bool flag = b == 0 || b == 42;
		bool flag2 = b2 == 0 || b2 == 42;
		Type typeOfCode = GetTypeOfCode(b);
		Type typeOfCode2 = GetTypeOfCode(b2);
		Type type = typeof(Dictionary<, >).MakeGenericType(typeOfCode, typeOfCode2);
		IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
		for (int i = 0; i < num; i++)
		{
			object key = Deserialize(din, flag ? ((byte)din.ReadByte()) : b);
			object value = Deserialize(din, flag2 ? ((byte)din.ReadByte()) : b2);
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	private bool DeserializeDictionaryArray(MemoryStream din, short size, out Array arrayResult)
	{
		byte keyTypeCode;
		byte valTypeCode;
		Type type = DeserializeDictionaryType(din, out keyTypeCode, out valTypeCode);
		arrayResult = Array.CreateInstance(type, size);
		for (short num = 0; num < size; num = (short)(num + 1))
		{
			IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
			if (dictionary == null)
			{
				return false;
			}
			short num2 = DeserializeShort(din);
			for (int i = 0; i < num2; i++)
			{
				object key;
				if (keyTypeCode != 0)
				{
					key = Deserialize(din, keyTypeCode);
				}
				else
				{
					byte type2 = (byte)din.ReadByte();
					key = Deserialize(din, type2);
				}
				object value;
				if (valTypeCode != 0)
				{
					value = Deserialize(din, valTypeCode);
				}
				else
				{
					byte type3 = (byte)din.ReadByte();
					value = Deserialize(din, type3);
				}
				dictionary.Add(key, value);
			}
			arrayResult.SetValue(dictionary, num);
		}
		return true;
	}

	private Type DeserializeDictionaryType(MemoryStream reader, out byte keyTypeCode, out byte valTypeCode)
	{
		keyTypeCode = (byte)reader.ReadByte();
		valTypeCode = (byte)reader.ReadByte();
		GpType gpType = (GpType)keyTypeCode;
		GpType gpType2 = (GpType)valTypeCode;
		Type type = ((gpType != 0) ? GetTypeOfCode(keyTypeCode) : typeof(object));
		Type type2 = ((gpType2 != 0) ? GetTypeOfCode(valTypeCode) : typeof(object));
		return typeof(Dictionary<, >).MakeGenericType(type, type2);
	}
}
