using System.Collections.Generic;
using System.Globalization;

public class BattleHistory
{
	public string opponentName;

	public string battleVideo;

	public string flag;

	public string season;

	public bool youWon;

	public List<string> creatures = new List<string>();

	public int currentLeague;

	public int bestLeague;

	public int bestLeagueSeasonID;

	public uint recordTime;

	public string opponentId;

	public string opponentFBId;

	public string opponentLeader;

	public BattleHistory(string _opponentName, string _battleVideo, string _flag, string _season, bool _youWon, List<string> _creatures, int _currentLeague, int _bestLeague, int _bestLeagueSeasonID, uint _recordTime, string _opponentId, string _opponentFBId, string _opponentLeader)
	{
		opponentName = _opponentName;
		battleVideo = _battleVideo;
		flag = _flag;
		season = _season;
		youWon = _youWon;
		creatures = _creatures;
		currentLeague = _currentLeague;
		bestLeague = _bestLeague;
		bestLeagueSeasonID = _bestLeagueSeasonID;
		recordTime = _recordTime;
		opponentId = _opponentId;
		opponentFBId = _opponentFBId;
		opponentLeader = _opponentLeader;
	}

	public static BattleHistory FromJSONDictionary(Dictionary<string, object> parameters)
	{
		List<string> list = new List<string>((string[])parameters["creatures"]);
		string text = (string)parameters["opponentName"];
		string text2 = (string)parameters["battleVideo"];
		string text3 = (string)parameters["flag"];
		string text4 = (string)parameters["season"];
		bool flag = (bool)parameters["youWon"];
		int num = int.Parse(parameters["currentLeague"].ToString(), CultureInfo.InvariantCulture);
		int num2 = int.Parse(parameters["bestLeague"].ToString(), CultureInfo.InvariantCulture);
		int num3 = int.Parse(parameters["bestLeagueSeasonID"].ToString(), CultureInfo.InvariantCulture);
		uint num4 = uint.Parse(parameters["recordTime"].ToString(), CultureInfo.InvariantCulture);
		string text5 = (string)parameters["opponentId"];
		string text6 = ((!parameters.ContainsKey("opponentFBId")) ? null : ((string)parameters["opponentFBId"]));
		string text7 = ((!parameters.ContainsKey("opponentLeader")) ? "Leader_Finn" : ((string)parameters["opponentLeader"]));
		return new BattleHistory(text, text2, text3, text4, flag, list, num, num2, num3, num4, text5, text6, text7);
	}

	public Dictionary<string, object> ToJSONDictionary()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("opponentName", opponentName);
		dictionary.Add("battleVideo", battleVideo);
		dictionary.Add("flag", flag);
		dictionary.Add("season", season);
		dictionary.Add("youWon", youWon);
		dictionary.Add("creatures", creatures);
		dictionary.Add("currentLeague", currentLeague);
		dictionary.Add("bestLeague", bestLeague);
		dictionary.Add("bestLeagueSeasonID", bestLeagueSeasonID);
		dictionary.Add("recordTime", recordTime);
		dictionary.Add("opponentId", opponentId);
		if (!string.IsNullOrEmpty(opponentFBId))
		{
			dictionary.Add("opponentFBId", opponentFBId);
		}
		dictionary.Add("opponentLeader", opponentLeader);
		return dictionary;
	}
}
