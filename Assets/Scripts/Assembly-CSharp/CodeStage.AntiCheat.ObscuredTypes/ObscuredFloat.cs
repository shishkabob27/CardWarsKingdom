using System;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredFloat : IFormattable, IEquatable<ObscuredFloat>
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntBytesUnion
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;
		}

		private static int cryptoKey = 230887;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private float fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredFloat(byte[] value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0f;
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public static int Encrypt(float value)
		{
			return Encrypt(value, cryptoKey);
		}

		public static int Encrypt(float value, int key)
		{
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= key;
			return floatIntBytesUnion.i;
		}

		private static byte[] InternalEncrypt(float value)
		{
			return InternalEncrypt(value, 0);
		}

		private static byte[] InternalEncrypt(float value, int key)
		{
			int num = key;
			if (num == 0)
			{
				num = cryptoKey;
			}
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.f = value;
			floatIntBytesUnion.i ^= num;
			return new byte[4] { floatIntBytesUnion.b1, floatIntBytesUnion.b2, floatIntBytesUnion.b3, floatIntBytesUnion.b4 };
		}

		public static float Decrypt(int value)
		{
			return Decrypt(value, cryptoKey);
		}

		public static float Decrypt(int value, int key)
		{
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.i = value ^ key;
			return floatIntBytesUnion.f;
		}

		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			float value = InternalDecrypt();
			currentCryptoKey = UnityEngine.Random.seed;
			hiddenValue = InternalEncrypt(value, currentCryptoKey);
		}

		public int GetEncrypted()
		{
			ApplyNewCryptoKey();
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.b1 = hiddenValue[0];
			floatIntBytesUnion.b2 = hiddenValue[1];
			floatIntBytesUnion.b3 = hiddenValue[2];
			floatIntBytesUnion.b4 = hiddenValue[3];
			return floatIntBytesUnion.i;
		}

		public void SetEncrypted(int encrypted)
		{
			inited = true;
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.i = encrypted;
			hiddenValue = new byte[4] { floatIntBytesUnion.b1, floatIntBytesUnion.b2, floatIntBytesUnion.b3, floatIntBytesUnion.b4 };
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private float InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0f);
				fakeValue = 0f;
				inited = true;
			}
			FloatIntBytesUnion floatIntBytesUnion = default(FloatIntBytesUnion);
			floatIntBytesUnion.b1 = hiddenValue[0];
			floatIntBytesUnion.b2 = hiddenValue[1];
			floatIntBytesUnion.b3 = hiddenValue[2];
			floatIntBytesUnion.b4 = hiddenValue[3];
			floatIntBytesUnion.i ^= currentCryptoKey;
			float f = floatIntBytesUnion.f;
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0f && Math.Abs(f - fakeValue) > ObscuredCheatingDetector.Instance.floatEpsilon)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return f;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredFloat))
			{
				return false;
			}
			return Equals((ObscuredFloat)obj);
		}

		public bool Equals(ObscuredFloat obj)
		{
			double num = obj.InternalDecrypt();
			double obj2 = InternalDecrypt();
			return num.Equals(obj2);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredFloat(float value)
		{
			ObscuredFloat result = new ObscuredFloat(InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator float(ObscuredFloat value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredFloat operator ++(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() + 1f;
			input.hiddenValue = InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredFloat operator --(ObscuredFloat input)
		{
			float value = input.InternalDecrypt() - 1f;
			input.hiddenValue = InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
