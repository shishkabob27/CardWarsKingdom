using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class OperationRequest
	{
		public byte OperationCode;

		public Dictionary<byte, object> Parameters;
	}
}
