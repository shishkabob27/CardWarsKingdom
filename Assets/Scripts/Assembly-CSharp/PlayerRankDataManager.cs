using System.Collections.Generic;
using System.IO;

public class PlayerRankDataManager : DataManager<PlayerRankData>
{
	private static PlayerRankDataManager _instance;

	public static PlayerRankDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine(SQSettings.CDN_URL, "Blueprints", "db_PlayerRank.json");
				_instance = new PlayerRankDataManager(path);
			}
			return _instance;
		}
	}

	public List<PlayerRankData> UnlockRanks { get; set; }

	public PlayerRankDataManager(string path)
	{
		base.FilePath = path;
	}

	protected override void PostLoad()
	{
		UnlockRanks = new List<PlayerRankData>();
		foreach (PlayerRankData item in GetDatabase())
		{
			if (item.UnlockType != 0)
			{
				UnlockRanks.Add(item);
			}
		}
	}

	public static void GetLabUnlockNameAndTexture(string unlockId, out string name, out string texture)
	{
		switch (unlockId)
		{
		case "Lab_Evo":
			name = "Creature Evo";
			texture = "UI/Icons_Buildings/UI_building_icon_lab";
			break;
		case "Lab_CardEquip":
			name = "Card Equipping";
			texture = "UI/Icons_Buildings/UI_building_icon_lab";
			break;
		case "Lab_Enhance":
			name = "Creature Enhancing";
			texture = "UI/Icons_Buildings/UI_building_icon_lab";
			break;
		default:
			name = string.Empty;
			texture = string.Empty;
			break;
		}
	}

	public int GetXpToUnlock(string unlockId)
	{
		foreach (PlayerRankData unlockRank in UnlockRanks)
		{
			if (unlockRank.UnlockId == unlockId)
			{
				return Singleton<PlayerInfoScript>.Instance.GetXpToReachRank(unlockRank.Level);
			}
		}
		return -1;
	}
}
