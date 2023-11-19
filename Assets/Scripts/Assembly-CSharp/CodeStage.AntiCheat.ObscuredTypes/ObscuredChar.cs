using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		private static char cryptoKey = '—';

		private char currentCryptoKey;

		private char hiddenValue;

		private char fakeValue;

		private bool inited;

		private ObscuredChar(char value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = value;
			fakeValue = '\0';
			inited = true;
		}

		public static void SetNewCryptoKey(char newKey)
		{
			cryptoKey = newKey;
		}

		public static char EncryptDecrypt(char value)
		{
			return EncryptDecrypt(value, '\0');
		}

		public static char EncryptDecrypt(char value, char key)
		{
			if (key == '\0')
			{
				return (char)(value ^ cryptoKey);
			}
			return (char)(value ^ key);
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
			char value = InternalDecrypt();
			currentCryptoKey = (char)(UnityEngine.Random.seed >> 24);
			hiddenValue = EncryptDecrypt(value, currentCryptoKey);
		}

		public char GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		public void SetEncrypted(char encrypted)
		{
			inited = true;
			hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				fakeValue = InternalDecrypt();
			}
		}

		private char InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = EncryptDecrypt('\0');
				fakeValue = '\0';
				inited = true;
			}
			char c = EncryptDecrypt(hiddenValue, currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && fakeValue != 0 && c != fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return c;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredChar))
			{
				return false;
			}
			return Equals((ObscuredChar)obj);
		}

		public bool Equals(ObscuredChar obj)
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

		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredChar operator ++(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() + 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredChar operator --(ObscuredChar input)
		{
			char value = (char)(input.InternalDecrypt() - 1);
			input.hiddenValue = EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
