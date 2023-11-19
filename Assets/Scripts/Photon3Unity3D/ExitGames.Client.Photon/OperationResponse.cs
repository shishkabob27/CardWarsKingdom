using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class OperationResponse
	{
		public byte OperationCode;

		public short ReturnCode;

		public string DebugMessage;

		public Dictionary<byte, object> Parameters;

		public object this[byte parameterCode]
		{
			get
			{
				Parameters.TryGetValue(parameterCode, out var value);
				return value;
			}
			set
			{
				Parameters[parameterCode] = value;
			}
		}

		public override string ToString()
		{
			return $"OperationResponse {OperationCode}: ReturnCode: {ReturnCode}.";
		}

		public string ToStringFull()
		{
			return string.Format("OperationResponse {0}: ReturnCode: {1} ({3}). Parameters: {2}", OperationCode, ReturnCode, SupportClass.DictionaryToString(Parameters), DebugMessage);
		}
	}
}
