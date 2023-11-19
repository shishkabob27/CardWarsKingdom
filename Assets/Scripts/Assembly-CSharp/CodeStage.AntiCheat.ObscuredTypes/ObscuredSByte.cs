using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredSByte : IFormattable, IEquatable<ObscuredSByte>
	{
		private static sbyte cryptoKey = 112;

		private sbyte currentCryptoKey;

		private sbyte hiddenValue;

		private sbyte fakeValue;

		private bool inited;

		private ObscuredSByte(sbyte value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = 0;
			inited = true;
		}

		public static void SetNewCryptoKey(sbyte newKey)
		{
			cryptoKey = newKey;
		}

		public static sbyte EncryptDecrypt(sbyte value)
		{
			return EncryptDecrypt(value, 0);
		}

		public static sbyte EncryptDecrypt(sbyte value, sbyte key)
		{
			if (key == 0)
			{
				return (sbyte)(value ^ cryptoKey);
			}
			return (sbyte)(value ^ key);
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
			sbyte value = InternalDecrypt();
			currentCryptoKey = (sbyte)(UnityEngine.Random.seed >> 24);
			hiddenValue = EncryptDecrypt(value, currentCryptoKey);
		}

		public sbyte GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(sbyte encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private sbyte InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt(0);
				fakeValue = 0;
				inited = true;
			}
			sbyte b = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0 && b != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredSByte))
			{
				return false;
			}
			return Equals((ObscuredSByte)obj);
		}

		public bool Equals(ObscuredSByte obj)
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

		public static implicit operator ObscuredSByte(sbyte value)
		{
			ObscuredSByte result = new ObscuredSByte(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator sbyte(ObscuredSByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredSByte operator ++(ObscuredSByte input)
		{
			sbyte value = (sbyte)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredSByte operator --(ObscuredSByte input)
		{
			sbyte value = (sbyte)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
