using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredULong : IFormattable, IEquatable<ObscuredULong>
	{
		private static ulong cryptoKey = 444443uL;

		private ulong currentCryptoKey;

		private ulong hiddenValue;

		private ulong fakeValue;

		private bool inited;

		private ObscuredULong(ulong value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0uL;
			inited = true;
		}

		public static void SetNewCryptoKey(ulong newKey)
		{
			cryptoKey = newKey;
		}

		public static ulong Encrypt(ulong value)
		{
			return Encrypt(value, 0uL);
		}

		public static ulong Decrypt(ulong value)
		{
			return Decrypt(value, 0uL);
		}

		public static ulong Encrypt(ulong value, ulong key)
		{
			if (key == 0L)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		public static ulong Decrypt(ulong value, ulong key)
		{
			if (key == 0L)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			ulong value = InternalDecrypt();
			currentCryptoKey = (ulong)UnityEngine.Random.seed;
			hiddenValue = Encrypt(value, currentCryptoKey);
		}

		public ulong GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(ulong encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private ulong InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0uL);
				fakeValue = 0uL;
				inited = true;
			}
			ulong num = Decrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0L && num != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredULong))
			{
				return false;
			}
			return Equals((ObscuredULong)obj);
		}

		public bool Equals(ObscuredULong obj)
		{
			if (currentCryptoKey == obj.currentCryptoKey)
			{
				return hiddenValue == obj.hiddenValue;
			}
			return Decrypt(hiddenValue, currentCryptoKey) == Decrypt(obj.hiddenValue, obj.currentCryptoKey);
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

		public static implicit operator ObscuredULong(ulong value)
		{
			ObscuredULong result = new ObscuredULong(Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ulong(ObscuredULong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredULong operator ++(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() + 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredULong operator --(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() - 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
