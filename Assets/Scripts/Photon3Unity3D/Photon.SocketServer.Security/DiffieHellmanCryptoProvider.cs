// Photon.SocketServer.Security.DiffieHellmanCryptoProvider
using System;
using System.Security.Cryptography;
using Photon.SocketServer.Numeric;
using Photon.SocketServer.Security;

internal class DiffieHellmanCryptoProvider : ICryptoProvider, IDisposable
{
	private static readonly BigInteger primeRoot = new BigInteger(OakleyGroups.Generator);

	private readonly BigInteger prime;

	private readonly BigInteger secret;

	private readonly BigInteger publicKey;

	private Rijndael crypto;

	private byte[] sharedKey;

	public bool IsInitialized
	{
		get
		{
			return crypto != null;
		}
	}

	public byte[] PublicKey
	{
		get
		{
			return publicKey.GetBytes();
		}
	}

	public DiffieHellmanCryptoProvider()
	{
		prime = new BigInteger(OakleyGroups.OakleyPrime768);
		secret = GenerateRandomSecret(160);
		publicKey = CalculatePublicKey();
	}

	public void DeriveSharedKey(byte[] otherPartyPublicKey)
	{
		BigInteger otherPartyPublicKey2 = new BigInteger(otherPartyPublicKey);
		BigInteger bigInteger = CalculateSharedKey(otherPartyPublicKey2);
		sharedKey = bigInteger.GetBytes();
		byte[] key;
		using (SHA256 sHA = new SHA256Managed())
		{
			key = sHA.ComputeHash(sharedKey);
		}
		crypto = new RijndaelManaged();
		crypto.Key = key;
		crypto.IV = new byte[16];
		crypto.Padding = PaddingMode.PKCS7;
	}

	public byte[] Encrypt(byte[] data)
	{
		return Encrypt(data, 0, data.Length);
	}

	public byte[] Encrypt(byte[] data, int offset, int count)
	{
		using (ICryptoTransform cryptoTransform = crypto.CreateEncryptor())
		{
			return cryptoTransform.TransformFinalBlock(data, offset, count);
		}
	}

	public byte[] Decrypt(byte[] data)
	{
		return Decrypt(data, 0, data.Length);
	}

	public byte[] Decrypt(byte[] data, int offset, int count)
	{
		using (ICryptoTransform cryptoTransform = crypto.CreateDecryptor())
		{
			return cryptoTransform.TransformFinalBlock(data, offset, count);
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
	}

	private BigInteger CalculatePublicKey()
	{
		return primeRoot.ModPow(secret, prime);
	}

	private BigInteger CalculateSharedKey(BigInteger otherPartyPublicKey)
	{
		return otherPartyPublicKey.ModPow(secret, prime);
	}

	private BigInteger GenerateRandomSecret(int secretLength)
	{
		BigInteger bigInteger;
		do
		{
			bigInteger = BigInteger.GenerateRandom(secretLength);
		}
		while (bigInteger >= prime - 1 || bigInteger == 0);
		return bigInteger;
	}
}
