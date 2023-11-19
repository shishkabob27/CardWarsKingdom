using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredByte : IFormattable, IEquatable<ObscuredByte>
	{
		private static byte cryptoKey = 244;

		private byte currentCryptoKey;

		private byte hiddenValue;

		private byte fakeValue;

		private bool inited;

		private ObscuredByte(byte value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(byte newKey)
		{
			cryptoKey = newKey;
		}

		public static byte EncryptDecrypt(byte value)
		{
			return EncryptDecrypt(value, 0);
		}

		public static byte EncryptDecrypt(byte value, byte key)
		{
			if (key == 0)
			{
				return (byte)(value ^ cryptoKey);
			}
			return (byte)(value ^ key);
		}

		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = EncryptDecrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			byte value = InternalDecrypt();
			currentCryptoKey = (byte)(UnityEngine.Random.seed >> 24);
			hiddenValue = EncryptDecrypt(value, currentCryptoKey);
		}

		public byte GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(byte encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private byte InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}
			byte b = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0 && b != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredByte))
			{
				return false;
			}
			return Equals((ObscuredByte)obj);
		}

		public bool Equals(ObscuredByte obj)
		{
			if (currentCryptoKey == obj.currentCryptoKey)
			{
				return hiddenValue == obj.hiddenValue;
			}
			return EncryptDecrypt(hiddenValue, currentCryptoKey) == EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredByte(byte value)
		{
			ObscuredByte result = new ObscuredByte(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator byte(ObscuredByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredByte operator ++(ObscuredByte input)
		{
			byte value = (byte)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredByte operator --(ObscuredByte input)
		{
			byte value = (byte)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
