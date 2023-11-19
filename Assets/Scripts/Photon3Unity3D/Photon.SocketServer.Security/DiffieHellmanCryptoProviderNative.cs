// Photon.SocketServer.Security.DiffieHellmanCryptoProviderNative
using System;
using System.Runtime.InteropServices;
using Photon.SocketServer.Security;

internal class DiffieHellmanCryptoProviderNative : ICryptoProvider, IDisposable
{
	private IntPtr cryptor;

	public bool IsInitialized
	{
		get
		{
			return egCryptorIsEncryptionAvailable(cryptor);
		}
	}

	public byte[] PublicKey
	{
		get
		{
			IntPtr key;
			int keySize;
			egCryptorPublicKey(cryptor, out key, out keySize);
			byte[] array = new byte[keySize];
			Marshal.Copy(key, array, 0, keySize);
			return array;
		}
	}

	[DllImport("PhotonCryptoPlugin")]
	internal static extern IntPtr egCryptorCreate();

	[DllImport("PhotonCryptoPlugin")]
	internal static extern int egCryptorPublicKey(IntPtr cryptor, out IntPtr key, out int keySize);

	[DllImport("PhotonCryptoPlugin")]
	internal static extern int egCryptorDeriveSharedKey(IntPtr cryptor, byte[] serverPublicKey, int keySize);

	[DllImport("PhotonCryptoPlugin")]
	internal static extern int egCryptorEncrypt(IntPtr cryptor, byte[] plainData, int plainDataOffset, int plainDataSize, out IntPtr encodedData, out int encodedDataSize);

	[DllImport("PhotonCryptoPlugin")]
	internal static extern int egCryptorDecrypt(IntPtr cryptor, byte[] encodedData, int encodedDataOffset, int encodedDataSize, out IntPtr plainData, out int plainDataSize);

	[DllImport("PhotonCryptoPlugin")]
	internal static extern bool egCryptorIsEncryptionAvailable(IntPtr cryptor);

	[DllImport("PhotonCryptoPlugin")]
	internal static extern void egCryptorDispose(IntPtr cryptor);

	public DiffieHellmanCryptoProviderNative()
	{
		cryptor = egCryptorCreate();
	}

	public void DeriveSharedKey(byte[] otherPartyPublicKey)
	{
		egCryptorDeriveSharedKey(cryptor, otherPartyPublicKey, otherPartyPublicKey.Length);
	}

	public byte[] Encrypt(byte[] data)
	{
		return Encrypt(data, 0, data.Length);
	}

	public byte[] Encrypt(byte[] data, int offset, int count)
	{
		IntPtr encodedData;
		int encodedDataSize;
		if (egCryptorEncrypt(cryptor, data, offset, count, out encodedData, out encodedDataSize) == 0)
		{
			byte[] array = new byte[encodedDataSize];
			Marshal.Copy(encodedData, array, 0, encodedDataSize);
			return array;
		}
		return null;
	}

	public byte[] Decrypt(byte[] data)
	{
		return Decrypt(data, 0, data.Length);
	}

	public byte[] Decrypt(byte[] data, int offset, int count)
	{
		IntPtr plainData;
		int plainDataSize;
		if (egCryptorDecrypt(cryptor, data, offset, count, out plainData, out plainDataSize) == 0)
		{
			byte[] array = new byte[plainDataSize];
			Marshal.Copy(plainData, array, 0, plainDataSize);
			return array;
		}
		return null;
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
			egCryptorDispose(cryptor);
		}
	}
}
