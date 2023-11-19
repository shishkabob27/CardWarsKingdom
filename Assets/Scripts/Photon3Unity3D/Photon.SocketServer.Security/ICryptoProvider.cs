using System;

namespace Photon.SocketServer.Security
{
	internal interface ICryptoProvider : IDisposable
	{
		bool IsInitialized { get; }

		byte[] PublicKey { get; }

		void DeriveSharedKey(byte[] otherPartyPublicKey);

		byte[] Encrypt(byte[] data);

		byte[] Encrypt(byte[] data, int offset, int count);

		byte[] Decrypt(byte[] data);

		byte[] Decrypt(byte[] data, int offset, int count);
	}
}
