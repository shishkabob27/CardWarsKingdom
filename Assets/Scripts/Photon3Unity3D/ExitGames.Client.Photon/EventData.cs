using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class EventData
	{
		public byte Code;

		public Dictionary<byte, object> Parameters;

		public object this[byte key]
		{
			get
			{
				Parameters.TryGetValue(key, out var value);
				return value;
			}
			set
			{
				Parameters[key] = value;
			}
		}

		public override string ToString()
		{
			return $"Event {Code.ToString()}.";
		}

		public string ToStringFull()
		{
			return $"Event {Code}: {SupportClass.DictionaryToString(Parameters)}";
		}
	}
}
