using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class RijndaelCrypto
{
	public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
	{
		MemoryStream memoryStream = new MemoryStream();
		Rijndael rijndael = Rijndael.Create();
		rijndael.Key = Key;
		rijndael.IV = IV;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
		cryptoStream.Write(clearData, 0, clearData.Length);
		cryptoStream.Close();
		return memoryStream.ToArray();
	}

	public static string Encrypt(string clearText, string Password)
	{
		byte[] bytes = Encoding.Unicode.GetBytes(clearText);
		PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(Password, new byte[13]
		{
			73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
			100, 101, 118
		});
		byte[] inArray = Encrypt(bytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
		return Convert.ToBase64String(inArray);
	}

	public static byte[] Encrypt(byte[] clearData, string Password)
	{
		PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(Password, new byte[13]
		{
			73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
			100, 101, 118
		});
		return Encrypt(clearData, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
	}

	public static void Encrypt(string fileIn, string fileOut, string Password)
	{
		FileStream fileStream = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
		FileStream stream = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(stream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
		int num = 4096;
		byte[] array = new byte[num];
		int num2;
		do
		{
			num2 = fileStream.Read(array, 0, num);
			cryptoStream.Write(array, 0, num2);
		}
		while (num2 != 0);
		cryptoStream.Close();
		fileStream.Close();
	}

	public static byte[] Decrypt(byte[] cipherData, string Password)
	{
		MemoryStream memoryStream = new MemoryStream();
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
		cryptoStream.Write(cipherData, 0, cipherData.Length);
		cryptoStream.Close();
		return memoryStream.ToArray();
	}

	public static string DecryptString(string cipherText, string Password)
	{
		byte[] cipherData = Convert.FromBase64String(cipherText);
		byte[] bytes = Decrypt(cipherData, Password);
		return Encoding.Unicode.GetString(bytes);
	}

	public static void Decrypt(string fileIn, string fileOut, string Password)
	{
		FileStream fileStream = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
		FileStream stream = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(stream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
		int num = 4096;
		byte[] array = new byte[num];
		int num2;
		do
		{
			num2 = fileStream.Read(array, 0, num);
			cryptoStream.Write(array, 0, num2);
		}
		while (num2 != 0);
		cryptoStream.Close();
		fileStream.Close();
	}

	public static string Decrypt(string fileIn, string Password)
	{
		FileStream fileStream = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
		MemoryStream memoryStream = new MemoryStream();
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
		int num = 4096;
		byte[] array = new byte[num];
		string empty = string.Empty;
		int num2;
		do
		{
			num2 = fileStream.Read(array, 0, num);
			cryptoStream.Write(array, 0, num2);
		}
		while (num2 != 0);
		cryptoStream.Close();
		fileStream.Close();
		byte[] bytes = memoryStream.ToArray();
		return Encoding.UTF8.GetString(bytes);
	}

	public static string DecryptBase64(string fileIn, string Password)
	{
		MemoryStream memoryStream = new MemoryStream();
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
		Encoding encoding = Encoding.GetEncoding("utf-8");
		string s = File.ReadAllText(fileIn, encoding);
		byte[] array = Convert.FromBase64String(s);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		byte[] array2 = memoryStream.ToArray();
		if (array2[0] == 239 && array2[1] == 187 && array2[2] == 191)
		{
			return Encoding.UTF8.GetString(array2, 3, array2.Length - 3);
		}
		return Encoding.UTF8.GetString(array2);
	}

	public static string DecryptBase64Text(string json, string Password)
	{
		MemoryStream memoryStream = new MemoryStream();
		Rijndael rijndael = Rijndael.Create();
		byte[] key;
		byte[] iv;
		GenerateKeyFromPassword(Password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
		rijndael.Key = key;
		rijndael.IV = iv;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
		byte[] array = Convert.FromBase64String(json);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		byte[] array2 = memoryStream.ToArray();
		if (array2[0] == 239 && array2[1] == 187 && array2[2] == 191)
		{
			return Encoding.UTF8.GetString(array2, 3, array2.Length - 3);
		}
		return Encoding.UTF8.GetString(array2);
	}

	private static void GenerateKeyFromPassword(string password, int keySize, out byte[] key, int blockSize, out byte[] iv)
	{
		byte[] bytes = Encoding.UTF8.GetBytes("saltmustbemorethan8bytes");
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, bytes);
		rfc2898DeriveBytes.IterationCount = 1000;
		key = rfc2898DeriveBytes.GetBytes(keySize / 8);
		iv = rfc2898DeriveBytes.GetBytes(blockSize / 8);
	}
}
