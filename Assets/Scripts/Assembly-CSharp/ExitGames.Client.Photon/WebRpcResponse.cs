using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class WebRpcResponse
	{
		public string Name { get; private set; }

		public int ReturnCode { get; private set; }

		public string DebugMessage { get; private set; }

		public Dictionary<string, object> Parameters { get; private set; }

		public WebRpcResponse(OperationResponse response)
		{
			object value;
			response.Parameters.TryGetValue(209, out value);
			Name = value as string;
			response.Parameters.TryGetValue(207, out value);
			ReturnCode = ((value == null) ? (-1) : ((byte)value));
			response.Parameters.TryGetValue(208, out value);
			Parameters = value as Dictionary<string, object>;
			response.Parameters.TryGetValue(206, out value);
			DebugMessage = value as string;
		}

		public string ToStringFull()
		{
			return string.Format("{0}={2}: {1} \"{3}\"", Name, SupportClass.DictionaryToString(Parameters), ReturnCode, DebugMessage);
		}
	}
}
