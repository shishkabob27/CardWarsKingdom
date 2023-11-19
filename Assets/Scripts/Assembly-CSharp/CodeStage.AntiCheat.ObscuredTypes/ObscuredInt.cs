using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredInt : IFormattable, IEquatable<ObscuredInt>
	{
		private static int cryptoKey = 444444;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private int hiddenValue;

		[SerializeField]
		private int fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredInt(int value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			cryptoKey = newKey;
		}

		public static int Encrypt(int value)
		{
			return Encrypt(value, 0);
		}

		public static int Encrypt(int value, int key)
		{
			if (key == 0)
			{
				return value ^ cryptoKey;
			}
			return value ^ key;
		}

		public static int Decrypt(int value)
		{
			return Decrypt(value, 0);
		}

		public static int Decrypt(int value, int key)
		{
			if (key == 0)
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
			hiddenValue = InternalDecrypt();
			currentCryptoKey = UnityEngine.Random.seed;
			hiddenValue = Encrypt(hiddenValue, currentCryptoKey);
		}

		public int GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(int encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private int InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = Encrypt(0);
				fakeValue = 0;
				inited = true;
			}
			int num = Decrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0 && num != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredInt))
			{
				return false;
			}
			return Equals((ObscuredInt)obj);
		}

		public bool Equals(ObscuredInt obj)
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

		public static implicit operator ObscuredInt(int value)
		{
			ObscuredInt result = new ObscuredInt(Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator int(ObscuredInt value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredUInt(ObscuredInt value)
		{
			return (uint)value.InternalDecrypt();
		}

		public static ObscuredInt operator ++(ObscuredInt input)
		{
			int value = input.InternalDecrypt() + 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredInt operator --(ObscuredInt input)
		{
			int value = input.InternalDecrypt() - 1;
			input.hiddenValue = Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
