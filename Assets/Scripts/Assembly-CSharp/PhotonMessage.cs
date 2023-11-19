using System;
using System.Collections.Generic;

[Serializable]
public class PhotonMessage : Dictionary<string, object>
{
	private const string BODY = "msg";

	private const string NAME = "1";

	private const string LEVEL = "2";

	private const string CURRENT_LEAGUE = "3";

	private const string BEST_LEAGUE = "4";

	private const string COUNTRY = "5";

	private const string FB_ID = "6";

	private const string USER_ID = "7";

	private const string LEADER = "8";

	private const string ZAPID = "9";

	private const string PORTRAIT = "10";

	private const string GACHA_CREATURE = "11";

	private const string DUNGEON = "12";

	private const string INVITE = "13";

	private const bool DEBUG_OUTPUT = true;

	public ChatMetaData Data { get; private set; }

	public string Body { get; private set; }

	private PhotonMessage()
	{
	}

	public PhotonMessage(string messageBody, PlayerInfoScript playerInfo, string countryCode, string gachaCreature, string dungeon, string inviteData)
	{
		Body = messageBody;
		string facebookId = string.Empty;
		if (playerInfo.IsFacebookLogin())
		{
			facebookId = Singleton<KFFSocialManager>.Instance.FBUser.UserId;
		}
		Data = new ChatMetaData();
		Data.UserId = playerInfo.GetPlayerCode();
		Data.Name = playerInfo.GetPlayerName();
		Data.Level = playerInfo.RankXpLevelData.mCurrentLevel;
		Data.FacebookId = facebookId;
		Data.CountryCode = countryCode;
		Data.CurrentLeague = playerInfo.SaveData.MultiplayerLevel;
		Data.BestLeague = playerInfo.SaveData.BestMultiplayerLevel;
		Data.Leader = playerInfo.GetCurrentLoadout().Leader.SelectedSkin.ID;
		Data.PortraitId = playerInfo.SaveData.SelectedPortrait.ID;
		Data.GachaCreature = gachaCreature;
		Data.Dungeon = dungeon;
		Data.InviteData = inviteData;
	}

	public PhotonMessage(string zapUserId)
	{
		Data = new ChatMetaData();
		Data.ZapUserId = zapUserId;
	}

	public Dictionary<string, object> Serialize()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("msg", Body);
		dictionary.Add("7", Data.UserId);
		dictionary.Add("1", Data.Name);
		dictionary.Add("6", Data.FacebookId);
		dictionary.Add("2", Data.Level);
		dictionary.Add("3", Data.CurrentLeague);
		dictionary.Add("4", Data.BestLeague);
		dictionary.Add("5", Data.CountryCode);
		dictionary.Add("8", Data.Leader);
		dictionary.Add("9", Data.ZapUserId);
		dictionary.Add("10", Data.PortraitId);
		dictionary.Add("11", Data.GachaCreature);
		dictionary.Add("12", Data.Dungeon);
		dictionary.Add("13", Data.InviteData);
		return dictionary;
	}

	public static bool TryDeserialize(object o, out PhotonMessage pm)
	{
		string text = o as string;
		if (text != null)
		{
			pm = new PhotonMessage();
			pm.Body = text;
			return true;
		}
		Dictionary<string, object> dictionary = o as Dictionary<string, object>;
		pm = new PhotonMessage();
		pm.Body = (string)dictionary["msg"];
		pm.Data = new ChatMetaData();
		pm.Data.UserId = (string)dictionary["7"];
		pm.Data.Name = (string)dictionary["1"];
		pm.Data.Level = (int)dictionary["2"];
		pm.Data.FacebookId = (string)dictionary["6"];
		pm.Data.CountryCode = (string)dictionary["5"];
		pm.Data.CurrentLeague = (int)dictionary["3"];
		pm.Data.BestLeague = (int)dictionary["4"];
		pm.Data.Leader = (string)dictionary["8"];
		pm.Data.ZapUserId = ((!dictionary.ContainsKey("9")) ? string.Empty : ((string)dictionary["9"]));
		pm.Data.PortraitId = ((!dictionary.ContainsKey("10")) ? null : ((string)dictionary["10"]));
		pm.Data.GachaCreature = ((!dictionary.ContainsKey("11")) ? null : ((string)dictionary["11"]));
		pm.Data.Dungeon = ((!dictionary.ContainsKey("12")) ? null : ((string)dictionary["12"]));
		pm.Data.InviteData = ((!dictionary.ContainsKey("13")) ? null : ((string)dictionary["13"]));
		return true;
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessage: Data={0}, Body={1}]", Data, Body);
	}
}
