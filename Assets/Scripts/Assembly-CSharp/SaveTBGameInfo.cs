using System.Collections.Generic;
using ExitGames.Client.Photon;

public class SaveTBGameInfo
{
	public int MyPlayerId;

	public string RoomName;

	public string DisplayName;

	public bool MyTurn;

	public Dictionary<string, object> AvailableProperties;

	public string ToStringFull()
	{
		return string.Format("\"{0}\"[{1}] {2} ({3})", RoomName, MyPlayerId, MyTurn, SupportClass.DictionaryToString(AvailableProperties));
	}
}
