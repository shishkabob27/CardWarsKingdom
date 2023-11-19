using System;

namespace ExitGames.Client.Photon
{
	public abstract class PhotonPing : IDisposable
	{
		public string DebugString = "";

		public bool Successful;

		protected internal bool GotResult;

		protected internal int PingLength = 13;

		protected internal byte[] PingBytes = new byte[13]
		{
			125, 125, 125, 125, 125, 125, 125, 125, 125, 125,
			125, 125, 0
		};

		protected internal byte PingId;

		public virtual bool StartPing(string ip)
		{
			throw new NotImplementedException();
		}

		public virtual bool Done()
		{
			throw new NotImplementedException();
		}

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}

		protected internal void Init()
		{
			GotResult = false;
			Successful = false;
			PingId = (byte)(Environment.TickCount % 255);
		}
	}
}
