using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class KFFCSUtils : MonoBehaviour
{
	public delegate void LockFunction(object param);

	private static object locker = new object();

	public static object CreateObjectByTypeName(string typename)
	{
		Type type = Type.GetType(typename);
		if (type != null)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}

	public static object CreateObjectByType(Type t)
	{
		if (t != null)
		{
			if (t != typeof(string))
			{
				return Activator.CreateInstance(t);
			}
			return string.Empty;
		}
		return null;
	}

	public static string UTF8ToString(byte[] bytes)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetString(bytes);
	}

	public static byte[] StringToUTF8(string str)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		return uTF8Encoding.GetBytes(str);
	}

	public static string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static string GetFirstWord(string str, out string rest, string delimiters = " \t\n\r")
	{
		int i = 0;
		string text = null;
		if (str == null || str.Length < 0)
		{
			rest = null;
			return null;
		}
		for (; i < str.Length && delimiters.IndexOf(str[i]) >= 0; i++)
		{
		}
		for (; i < str.Length && delimiters.IndexOf(str[i]) < 0; i++)
		{
			if (text == null)
			{
				text = string.Empty;
			}
			text += str[i];
		}
		for (; i < str.Length && delimiters.IndexOf(str[i]) >= 0; i++)
		{
		}
		rest = str.Substring(i);
		return text;
	}

	public static void LockAndCallFunction(LockFunction f, object param)
	{
		if (f != null)
		{
			lock (locker)
			{
				f(param);
			}
		}
	}

	public static string GetLanguageCode()
	{
		return "en";
	}

	public static bool IsOnPhoneCall()
	{
		return false;
	}

	public static int GetInstalledDate()
	{
		long installedDate = 0L;
		return (int)installedDate;
	}

	public static bool isJB()
	{
		return false;
	}
}
