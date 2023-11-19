using System;
using System.Text;
using CodeStage.AntiCheat.Utils;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public static class ObscuredPrefs
	{
		private enum DataType : byte
		{
			Int = 5,
			UInt = 10,
			String = 15,
			Float = 20,
			Double = 25,
			Long = 30,
			Bool = 35,
			ByteArray = 40,
			Vector2 = 45,
			Vector3 = 50,
			Quaternion = 55,
			Color = 60,
			Rect = 65
		}

		public enum DeviceLockLevel : byte
		{
			None,
			Soft,
			Strict
		}

		private const byte VERSION = 2;

		private const string RAW_NOT_FOUND = "{not_found}";

		private const string DATA_SEPARATOR = "|";

		private const char DEPRECATED_RAW_SEPARATOR = ':';

		private static string encryptionKey = "e806f6";

		private static bool foreignSavesReported;

		private static string deviceId;

		private static uint deviceIdHash;

		public static Action onAlterationDetected;

		public static bool preservePlayerPrefs;

		public static Action onPossibleForeignSavesDetected;

		public static DeviceLockLevel lockToDevice;

		public static bool readForeignSaves;

		public static bool emergencyMode;

		private static string deprecatedDeviceId;

		public static string DeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(deviceId))
				{
					deviceId = GetDeviceId();
				}
				return deviceId;
			}
			set
			{
				deviceId = value;
				deviceIdHash = CalculateChecksum(deviceId);
			}
		}

		[Obsolete("This property is obsolete, please use DeviceId instead.")]
		internal static string DeviceID
		{
			get
			{
				if (string.IsNullOrEmpty(deviceId))
				{
					deviceId = GetDeviceId();
				}
				return deviceId;
			}
			set
			{
				deviceId = value;
				deviceIdHash = CalculateChecksum(deviceId);
			}
		}

		private static uint DeviceIdHash
		{
			get
			{
				if (deviceIdHash == 0)
				{
					deviceIdHash = CalculateChecksum(DeviceId);
				}
				return deviceIdHash;
			}
		}

		private static string DeprecatedDeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(deprecatedDeviceId))
				{
					deprecatedDeviceId = DeprecatedCalculateChecksum(DeviceId);
				}
				return deprecatedDeviceId;
			}
		}

		public static void ForceLockToDeviceInit()
		{
			if (string.IsNullOrEmpty(deviceId))
			{
				deviceId = GetDeviceId();
				deviceIdHash = CalculateChecksum(deviceId);
			}
		}

		public static void SetNewCryptoKey(string newKey)
		{
			encryptionKey = newKey;
			deviceIdHash = CalculateChecksum(deviceId);
		}

		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptIntValue(key, value));
		}

		public static int GetInt(string key)
		{
			return GetInt(key, 0);
		}

		public static int GetInt(string key, int defaultValue)
		{
			string text = EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				int @int = PlayerPrefs.GetInt(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetInt(key, @int);
					PlayerPrefs.DeleteKey(key);
				}
				return @int;
			}
			string encryptedPrefsString = GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptIntValue(string key, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Int);
		}

		private static int DecryptIntValue(string key, string encryptedInput, int defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int result;
				int.TryParse(text, out result);
				SetInt(key, result);
				return result;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToInt32(array, 0);
		}

		public static void SetUInt(string key, uint value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptUIntValue(key, value));
		}

		public static uint GetUInt(string key)
		{
			return GetUInt(key, 0u);
		}

		public static uint GetUInt(string key, uint defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptUIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptUIntValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.UInt);
		}

		private static uint DecryptUIntValue(string key, string encryptedInput, uint defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				uint result;
				uint.TryParse(text, out result);
				SetUInt(key, result);
				return result;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToUInt32(array, 0);
		}

		public static void SetString(string key, string value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptStringValue(key, value));
		}

		public static string GetString(string key)
		{
			return GetString(key, string.Empty);
		}

		public static string GetString(string key, string defaultValue)
		{
			string text = EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetString(key, @string);
					PlayerPrefs.DeleteKey(key);
				}
				return @string;
			}
			string encryptedPrefsString = GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptStringValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptStringValue(string key, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return EncryptData(key, bytes, DataType.String);
		}

		private static string DecryptStringValue(string key, string encryptedInput, string defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				SetString(key, text);
				return text;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return Encoding.UTF8.GetString(array, 0, array.Length);
		}

		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptFloatValue(key, value));
		}

		public static float GetFloat(string key)
		{
			return GetFloat(key, 0f);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			string text = EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				float @float = PlayerPrefs.GetFloat(key, defaultValue);
				if (!preservePlayerPrefs)
				{
					SetFloat(key, @float);
					PlayerPrefs.DeleteKey(key);
				}
				return @float;
			}
			string encryptedPrefsString = GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptFloatValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptFloatValue(string key, float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Float);
		}

		private static float DecryptFloatValue(string key, string encryptedInput, float defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				float result;
				float.TryParse(text, out result);
				SetFloat(key, result);
				return result;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToSingle(array, 0);
		}

		public static void SetDouble(string key, double value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptDoubleValue(key, value));
		}

		public static double GetDouble(string key)
		{
			return GetDouble(key, 0.0);
		}

		public static double GetDouble(string key, double defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptDoubleValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptDoubleValue(string key, double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Double);
		}

		private static double DecryptDoubleValue(string key, string encryptedInput, double defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				double result;
				double.TryParse(text, out result);
				SetDouble(key, result);
				return result;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToDouble(array, 0);
		}

		public static void SetLong(string key, long value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptLongValue(key, value));
		}

		public static long GetLong(string key)
		{
			return GetLong(key, 0L);
		}

		public static long GetLong(string key, long defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptLongValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptLongValue(string key, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Long);
		}

		private static long DecryptLongValue(string key, string encryptedInput, long defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				long result;
				long.TryParse(text, out result);
				SetLong(key, result);
				return result;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToInt64(array, 0);
		}

		public static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptBoolValue(key, value));
		}

		public static bool GetBool(string key)
		{
			return GetBool(key, false);
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptBoolValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptBoolValue(string key, bool value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Bool);
		}

		private static bool DecryptBoolValue(string key, string encryptedInput, bool defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int result;
				int.TryParse(text, out result);
				SetBool(key, result == 1);
				return result == 1;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return defaultValue;
			}
			return BitConverter.ToBoolean(array, 0);
		}

		public static void SetByteArray(string key, byte[] value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptByteArrayValue(key, value));
		}

		public static byte[] GetByteArray(string key)
		{
			return GetByteArray(key, 0, 0);
		}

		public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return ConstructByteArray(defaultValue, defaultLength);
			}
			return DecryptByteArrayValue(key, encryptedPrefsString, defaultValue, defaultLength);
		}

		private static string EncryptByteArrayValue(string key, byte[] value)
		{
			return EncryptData(key, value, DataType.ByteArray);
		}

		private static byte[] DecryptByteArrayValue(string key, string encryptedInput, byte defaultValue, int defaultLength)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return ConstructByteArray(defaultValue, defaultLength);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				SetByteArray(key, bytes);
				return bytes;
			}
			byte[] array = DecryptData(key, encryptedInput);
			if (array == null)
			{
				return ConstructByteArray(defaultValue, defaultLength);
			}
			return array;
		}

		private static byte[] ConstructByteArray(byte value, int length)
		{
			byte[] array = new byte[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		public static void SetVector2(string key, Vector2 value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptVector2Value(key, value));
		}

		public static Vector2 GetVector2(string key)
		{
			return GetVector2(key, Vector2.zero);
		}

		public static Vector2 GetVector2(string key, Vector2 defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptVector2Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptVector2Value(string key, Vector2 value)
		{
			byte[] array = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			return EncryptData(key, array, DataType.Vector2);
		}

		private static Vector2 DecryptVector2Value(string key, string encryptedInput, Vector2 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split("|"[0]);
				float result;
				float.TryParse(array[0], out result);
				float result2;
				float.TryParse(array[1], out result2);
				Vector2 vector = new Vector2(result, result2);
				SetVector2(key, vector);
				return vector;
			}
			byte[] array2 = DecryptData(key, encryptedInput);
			if (array2 == null)
			{
				return defaultValue;
			}
			Vector2 result3 = default(Vector2);
			result3.x = BitConverter.ToSingle(array2, 0);
			result3.y = BitConverter.ToSingle(array2, 4);
			return result3;
		}

		public static void SetVector3(string key, Vector3 value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptVector3Value(key, value));
		}

		public static Vector3 GetVector3(string key)
		{
			return GetVector3(key, Vector3.zero);
		}

		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptVector3Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptVector3Value(string key, Vector3 value)
		{
			byte[] array = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			return EncryptData(key, array, DataType.Vector3);
		}

		private static Vector3 DecryptVector3Value(string key, string encryptedInput, Vector3 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split("|"[0]);
				float result;
				float.TryParse(array[0], out result);
				float result2;
				float.TryParse(array[1], out result2);
				float result3;
				float.TryParse(array[2], out result3);
				Vector3 vector = new Vector3(result, result2, result3);
				SetVector3(key, vector);
				return vector;
			}
			byte[] array2 = DecryptData(key, encryptedInput);
			if (array2 == null)
			{
				return defaultValue;
			}
			Vector3 result4 = default(Vector3);
			result4.x = BitConverter.ToSingle(array2, 0);
			result4.y = BitConverter.ToSingle(array2, 4);
			result4.z = BitConverter.ToSingle(array2, 8);
			return result4;
		}

		public static void SetQuaternion(string key, Quaternion value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptQuaternionValue(key, value));
		}

		public static Quaternion GetQuaternion(string key)
		{
			return GetQuaternion(key, Quaternion.identity);
		}

		public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptQuaternionValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptQuaternionValue(string key, Quaternion value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, array, 12, 4);
			return EncryptData(key, array, DataType.Quaternion);
		}

		private static Quaternion DecryptQuaternionValue(string key, string encryptedInput, Quaternion defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split("|"[0]);
				float result;
				float.TryParse(array[0], out result);
				float result2;
				float.TryParse(array[1], out result2);
				float result3;
				float.TryParse(array[2], out result3);
				float result4;
				float.TryParse(array[3], out result4);
				Quaternion quaternion = new Quaternion(result, result2, result3, result4);
				SetQuaternion(key, quaternion);
				return quaternion;
			}
			byte[] array2 = DecryptData(key, encryptedInput);
			if (array2 == null)
			{
				return defaultValue;
			}
			Quaternion result5 = default(Quaternion);
			result5.x = BitConverter.ToSingle(array2, 0);
			result5.y = BitConverter.ToSingle(array2, 4);
			result5.z = BitConverter.ToSingle(array2, 8);
			result5.w = BitConverter.ToSingle(array2, 12);
			return result5;
		}

		public static void SetColor(string key, Color32 value)
		{
			uint value2 = (uint)((value.a << 24) | (value.r << 16) | (value.g << 8) | value.b);
			PlayerPrefs.SetString(EncryptKey(key), EncryptColorValue(key, value2));
		}

		public static Color32 GetColor(string key)
		{
			return GetColor(key, new Color32(0, 0, 0, 1));
		}

		public static Color32 GetColor(string key, Color32 defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return defaultValue;
			}
			uint num = DecryptUIntValue(key, encryptedPrefsString, 16777216u);
			byte a = (byte)(num >> 24);
			byte r = (byte)(num >> 16);
			byte g = (byte)(num >> 8);
			byte b = (byte)num;
			return new Color32(r, g, b, a);
		}

		private static string EncryptColorValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return EncryptData(key, bytes, DataType.Color);
		}

		public static void SetRect(string key, Rect value)
		{
			PlayerPrefs.SetString(EncryptKey(key), EncryptRectValue(key, value));
		}

		public static Rect GetRect(string key)
		{
			return GetRect(key, new Rect(0f, 0f, 0f, 0f));
		}

		public static Rect GetRect(string key, Rect defaultValue)
		{
			string encryptedPrefsString = GetEncryptedPrefsString(key, EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? DecryptRectValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptRectValue(string key, Rect value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.width), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.height), 0, array, 12, 4);
			return EncryptData(key, array, DataType.Rect);
		}

		private static Rect DecryptRectValue(string key, string encryptedInput, Rect defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split("|"[0]);
				float result;
				float.TryParse(array[0], out result);
				float result2;
				float.TryParse(array[1], out result2);
				float result3;
				float.TryParse(array[2], out result3);
				float result4;
				float.TryParse(array[3], out result4);
				Rect rect = new Rect(result, result2, result3, result4);
				SetRect(key, rect);
				return rect;
			}
			byte[] array2 = DecryptData(key, encryptedInput);
			if (array2 == null)
			{
				return defaultValue;
			}
			Rect result5 = default(Rect);
			result5.x = BitConverter.ToSingle(array2, 0);
			result5.y = BitConverter.ToSingle(array2, 4);
			result5.width = BitConverter.ToSingle(array2, 8);
			result5.height = BitConverter.ToSingle(array2, 12);
			return result5;
		}

		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(EncryptKey(key));
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(EncryptKey(key));
			if (!preservePlayerPrefs)
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}

		private static string GetEncryptedPrefsString(string key, string encryptedKey)
		{
			string @string = PlayerPrefs.GetString(encryptedKey, "{not_found}");
			if (!(@string == "{not_found}") || PlayerPrefs.HasKey(key))
			{
			}
			return @string;
		}

		private static string EncryptKey(string key)
		{
			key = ObscuredString.EncryptDecrypt(key, encryptionKey);
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
			return key;
		}

		private static string EncryptData(string key, byte[] cleanBytes, DataType type)
		{
			int num = cleanBytes.Length;
			byte[] src = EncryptDecryptBytes(cleanBytes, num, key + encryptionKey);
			uint num2 = xxHash.CalculateHash(cleanBytes, num, 0u);
			byte[] src2 = new byte[4]
			{
				(byte)(num2 & 0xFFu),
				(byte)((num2 >> 8) & 0xFFu),
				(byte)((num2 >> 16) & 0xFFu),
				(byte)((num2 >> 24) & 0xFFu)
			};
			byte[] array = null;
			int num3;
			if (lockToDevice != 0)
			{
				num3 = num + 11;
				uint num4 = DeviceIdHash;
				array = new byte[4]
				{
					(byte)(num4 & 0xFFu),
					(byte)((num4 >> 8) & 0xFFu),
					(byte)((num4 >> 16) & 0xFFu),
					(byte)((num4 >> 24) & 0xFFu)
				};
			}
			else
			{
				num3 = num + 7;
			}
			byte[] array2 = new byte[num3];
			Buffer.BlockCopy(src, 0, array2, 0, num);
			if (array != null)
			{
				Buffer.BlockCopy(array, 0, array2, num, 4);
			}
			array2[num3 - 7] = (byte)type;
			array2[num3 - 6] = 2;
			array2[num3 - 5] = (byte)lockToDevice;
			Buffer.BlockCopy(src2, 0, array2, num3 - 4, 4);
			return Convert.ToBase64String(array2);
		}

		private static byte[] DecryptData(string key, string encryptedInput)
		{
			//Discarded unreachable code: IL_001a
			byte[] array;
			try
			{
				array = Convert.FromBase64String(encryptedInput);
			}
			catch (Exception)
			{
				SavesTampered();
				return null;
			}
			if (array.Length <= 0)
			{
				SavesTampered();
				return null;
			}
			int num = array.Length;
			byte b = array[num - 6];
			if (b != 2)
			{
				SavesTampered();
				return null;
			}
			DeviceLockLevel deviceLockLevel = (DeviceLockLevel)array[num - 5];
			byte[] array2 = new byte[4];
			Buffer.BlockCopy(array, num - 4, array2, 0, 4);
			uint num2 = (uint)(array2[0] | (array2[1] << 8) | (array2[2] << 16) | (array2[3] << 24));
			uint num3 = 0u;
			int num4;
			if (deviceLockLevel != 0)
			{
				num4 = num - 11;
				if (lockToDevice != 0)
				{
					byte[] array3 = new byte[4];
					Buffer.BlockCopy(array, num4, array3, 0, 4);
					num3 = (uint)(array3[0] | (array3[1] << 8) | (array3[2] << 16) | (array3[3] << 24));
				}
			}
			else
			{
				num4 = num - 7;
			}
			byte[] array4 = new byte[num4];
			Buffer.BlockCopy(array, 0, array4, 0, num4);
			byte[] array5 = EncryptDecryptBytes(array4, num4, key + encryptionKey);
			uint num5 = xxHash.CalculateHash(array5, num4, 0u);
			if (num5 != num2)
			{
				SavesTampered();
				return null;
			}
			if (lockToDevice == DeviceLockLevel.Strict && num3 == 0 && !emergencyMode && !readForeignSaves)
			{
				return null;
			}
			if (num3 != 0 && !emergencyMode)
			{
				uint num6 = DeviceIdHash;
				if (num3 != num6)
				{
					PossibleForeignSavesDetected();
					if (!readForeignSaves)
					{
						return null;
					}
				}
			}
			return array5;
		}

		private static uint CalculateChecksum(string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input + encryptionKey);
			return xxHash.CalculateHash(bytes, bytes.Length, 0u);
		}

		private static void SavesTampered()
		{
			if (onAlterationDetected != null)
			{
				onAlterationDetected();
				onAlterationDetected = null;
			}
		}

		private static void PossibleForeignSavesDetected()
		{
			if (onPossibleForeignSavesDetected != null && !foreignSavesReported)
			{
				foreignSavesReported = true;
				onPossibleForeignSavesDetected();
			}
		}

		private static string GetDeviceId()
		{
			return string.Empty;
		}

		private static byte[] EncryptDecryptBytes(byte[] bytes, int dataLength, string key)
		{
			int length = key.Length;
			byte[] array = new byte[dataLength];
			for (int i = 0; i < dataLength; i++)
			{
				array[i] = (byte)(bytes[i] ^ key[i % length]);
			}
			return array;
		}

		private static string DeprecatedDecryptValue(string value)
		{
			//Discarded unreachable code: IL_004c
			string[] array = value.Split(':');
			if (array.Length < 2)
			{
				SavesTampered();
				return string.Empty;
			}
			string text = array[0];
			string text2 = array[1];
			byte[] array2;
			try
			{
				array2 = Convert.FromBase64String(text);
			}
			catch
			{
				SavesTampered();
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(array2, 0, array2.Length);
			string result = ObscuredString.EncryptDecrypt(@string, encryptionKey);
			if (array.Length == 3)
			{
				if (text2 != DeprecatedCalculateChecksum(text + DeprecatedDeviceId))
				{
					SavesTampered();
				}
			}
			else if (array.Length == 2)
			{
				if (text2 != DeprecatedCalculateChecksum(text))
				{
					SavesTampered();
				}
			}
			else
			{
				SavesTampered();
			}
			if (lockToDevice != 0 && !emergencyMode)
			{
				if (array.Length >= 3)
				{
					string text3 = array[2];
					if (text3 != DeprecatedDeviceId)
					{
						if (!readForeignSaves)
						{
							result = string.Empty;
						}
						PossibleForeignSavesDetected();
					}
				}
				else if (lockToDevice == DeviceLockLevel.Strict)
				{
					if (!readForeignSaves)
					{
						result = string.Empty;
					}
					PossibleForeignSavesDetected();
				}
				else if (text2 != DeprecatedCalculateChecksum(text))
				{
					if (!readForeignSaves)
					{
						result = string.Empty;
					}
					PossibleForeignSavesDetected();
				}
			}
			return result;
		}

		private static string DeprecatedCalculateChecksum(string input)
		{
			int num = 0;
			byte[] bytes = Encoding.UTF8.GetBytes(input + encryptionKey);
			int num2 = bytes.Length;
			int num3 = encryptionKey.Length ^ 0x40;
			for (int i = 0; i < num2; i++)
			{
				byte b = bytes[i];
				num += b + b * (i + num3) % 3;
			}
			return num.ToString("X2");
		}
	}
}
