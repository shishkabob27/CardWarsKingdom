using System.Collections.Generic;
using System.Text;
using Allies;
using MiniJSON;

public class HelperItem
{
	public string HelperID;

	public string HelperName;

	public int IsAlly;

	public int SinceLastActiveTime;

	public int HelperRank;

	public AllyData HelperAllyData;

	public InventorySlotItem HelperCreature;

	public bool IsSelected;

	public bool OnlineStatus;

	public bool Fake;

	public bool Favorite { get; set; }

	public Loadout HelperLoadout { get; set; }

	public HelperItem(int isAlly)
	{
		IsAlly = isAlly;
	}

	public HelperItem(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public HelperItem(string userID)
	{
		HelperID = userID;
	}

	public HelperItem(string userName, string userID)
	{
		HelperName = userName;
		HelperID = userID;
	}

	public HelperItem(AllyData ally)
	{
		HelperID = ally.Id;
		HelperName = ally.Name;
		SinceLastActiveTime = ally.Sincelastactivedate;
		HelperCreature = Singleton<PlayerInfoScript>.Instance.DeserializeHelperCreature(ally.HelperCreature);
		HelperCreature.Creature.FromOtherPlayer = true;
		if (HelperCreature != null)
		{
			HelperID = ally.Id;
			HelperRank = ally.Level;
			IsAlly = ally.IsAlly;
			HelperAllyData = ally;
		}
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("ID", HelperID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Name", HelperName) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("IsAlly", IsAlly) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Rank", HelperRank) + ",");
		string val = HelperCreature.Creature.Serialize();
		stringBuilder.Append(PlayerInfoScript.MakeJS("HelperCreature", val) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		HelperID = TFUtils.LoadString(dict, "ID", string.Empty);
		HelperName = TFUtils.LoadString(dict, "Name", string.Empty);
		IsAlly = TFUtils.LoadInt(dict, "IsAlly", 0);
		HelperRank = TFUtils.LoadInt(dict, "Rank", 0);
		string json = TFUtils.LoadString(dict, "HelperCreature", string.Empty);
		Dictionary<string, object> dict2 = Json.Deserialize(json) as Dictionary<string, object>;
		HelperCreature = new InventorySlotItem(new CreatureItem(dict2, true));
	}
}
