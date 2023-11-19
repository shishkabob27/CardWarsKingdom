// ExitGames.Client.Photon.OperationResponse
using System.Collections.Generic;
using ExitGames.Client.Photon;

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
			object value;
			Parameters.TryGetValue(parameterCode, out value);
			return value;
		}
		set
		{
			Parameters[parameterCode] = value;
		}
	}

	public override string ToString()
	{
		return string.Format("OperationResponse {0}: ReturnCode: {1}.", OperationCode, ReturnCode);
	}

	public string ToStringFull()
	{
		return string.Format("OperationResponse {0}: ReturnCode: {1} ({3}). Parameters: {2}", OperationCode, ReturnCode, SupportClass.DictionaryToString(Parameters), DebugMessage);
	}
}
