using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredUShort : IFormattable, IEquatable<ObscuredUShort>
	{
		private static ushort cryptoKey = 224;

		private ushort currentCryptoKey;

		private ushort hiddenValue;

		private ushort fakeValue;

		private bool inited;

		private ObscuredUShort(ushort value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(ushort newKey)
		{
			cryptoKey = newKey;
		}

		public static ushort EncryptDecrypt(ushort value)
		{
			return EncryptDecrypt(value, 0);
		}

		public static ushort EncryptDecrypt(ushort value, ushort key)
		{
			if (key == 0)
			{
				return (ushort)(value ^ cryptoKey);
			}
			return (ushort)(value ^ key);
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
			ushort value = InternalDecrypt();
			currentCryptoKey = (ushort)UnityEngine.Random.seed;
			hiddenValue = EncryptDecrypt(value, currentCryptoKey);
		}

		public ushort GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(ushort encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private ushort InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}
			ushort num = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0 && num != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredUShort))
			{
				return false;
			}
			return Equals((ObscuredUShort)obj);
		}

		public bool Equals(ObscuredUShort obj)
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

		public static implicit operator ObscuredUShort(ushort value)
		{
			ObscuredUShort result = new ObscuredUShort(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ushort(ObscuredUShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUShort operator ++(ObscuredUShort input)
		{
			ushort value = (ushort)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredUShort operator --(ObscuredUShort input)
		{
			ushort value = (ushort)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
