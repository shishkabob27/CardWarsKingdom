using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class Protocol
	{
		public static readonly IProtocol GpBinaryV16 = new Protocol16();

		public static readonly IProtocol GpBinaryV17;

		public static readonly IProtocol ProtocolDefault = GpBinaryV16;

		internal static readonly Dictionary<Type, CustomType> TypeDict = new Dictionary<Type, CustomType>();

		internal static readonly Dictionary<byte, CustomType> CodeDict = new Dictionary<byte, CustomType>();

		public static bool TryRegisterType(Type type, byte typeCode, SerializeMethod serializeFunction, DeserializeMethod deserializeFunction)
		{
			if (CodeDict.ContainsKey(typeCode) || TypeDict.ContainsKey(type))
			{
				return false;
			}
			CustomType value = new CustomType(type, typeCode, serializeFunction, deserializeFunction);
			CodeDict.Add(typeCode, value);
			TypeDict.Add(type, value);
			return true;
		}

		public static bool TryRegisterType(Type type, byte typeCode, SerializeStreamMethod serializeFunction, DeserializeStreamMethod deserializeFunction)
		{
			if (CodeDict.ContainsKey(typeCode) || TypeDict.ContainsKey(type))
			{
				return false;
			}
			CustomType value = new CustomType(type, typeCode, serializeFunction, deserializeFunction);
			CodeDict.Add(typeCode, value);
			TypeDict.Add(type, value);
			return true;
		}

		public static byte[] Serialize(object obj)
		{
			return ProtocolDefault.Serialize(obj);
		}

		public static object Deserialize(byte[] serializedData)
		{
			return ProtocolDefault.Deserialize(serializedData);
		}

		public static void Serialize(short value, byte[] target, ref int targetOffset)
		{
			ProtocolDefault.Serialize(value, target, ref targetOffset);
		}

		public static void Serialize(int value, byte[] target, ref int targetOffset)
		{
			ProtocolDefault.Serialize(value, target, ref targetOffset);
		}

		public static void Serialize(float value, byte[] target, ref int targetOffset)
		{
			ProtocolDefault.Serialize(value, target, ref targetOffset);
		}

		public static void Deserialize(out int value, byte[] source, ref int offset)
		{
			ProtocolDefault.Deserialize(out value, source, ref offset);
		}

		public static void Deserialize(out short value, byte[] source, ref int offset)
		{
			ProtocolDefault.Deserialize(out value, source, ref offset);
		}

		public static void Deserialize(out float value, byte[] source, ref int offset)
		{
			ProtocolDefault.Deserialize(out value, source, ref offset);
		}
	}
}
