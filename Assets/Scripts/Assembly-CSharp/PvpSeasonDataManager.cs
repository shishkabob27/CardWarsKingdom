using System;
using System.Collections.Generic;
using System.IO;

public class PvpSeasonDataManager : DataManager<PvpSeasonData>
{
	private static PvpSeasonDataManager _instance;

	public static PvpSeasonDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				string path = Path.Combine("Blueprints", "db_PVPSeasons.json");
				_instance = new PvpSeasonDataManager(path);
			}
			return _instance;
		}
	}

	public PvpSeasonDataManager(string path)
	{
		base.FilePath = path;
		AddDependency(CardBackDataManager.Instance);
		AddDependency(PlayerTitleDataManager.Instance);
		AddDependency(PlayerBadgeDataManager.Instance);
		AddDependency(PvpRankDataManager.Instance);
	}

	protected override void ParseRows(List<object> jlist)
	{
		PvpSeasonData pvpSeasonData = null;
		foreach (object item in jlist)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
			string text = TFUtils.LoadString(dictionary, "Season", string.Empty);
			if (pvpSeasonData == null || text != "^")
			{
				PvpSeasonData previousSeason = pvpSeasonData;
				pvpSeasonData = new PvpSeasonData();
				pvpSeasonData.Populate(dictionary);
				pvpSeasonData.PreviousSeason = previousSeason;
				Database.Add(pvpSeasonData.ID, pvpSeasonData);
				DatabaseArray.Add(pvpSeasonData);
			}
			else
			{
				pvpSeasonData.AddRewardRow(dictionary);
			}
		}
	}

	public PvpSeasonData GetCurrentSeason()
	{
		DateTime serverTime = TFUtils.ServerTime;
		foreach (PvpSeasonData item in DatabaseArray)
		{
			if (item.EndDate > serverTime)
			{
				return item;
			}
		}
		if (DatabaseArray.Count > 0)
		{
			return DatabaseArray[DatabaseArray.Count - 1];
		}
		return null;
	}

	public string RankName(int rank, bool inShouldUseShortName = true)
	{
		string key = "!!UNKNOWN";
		switch (rank)
		{
		case 1:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_01" : "!!LEAGUE_RANK_SHORT_01");
			break;
		case 2:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_02" : "!!LEAGUE_RANK_SHORT_02");
			break;
		case 3:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_03" : "!!LEAGUE_RANK_SHORT_03");
			break;
		case 4:
		case 5:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_04" : "!!LEAGUE_RANK_SHORT_04");
			break;
		case 6:
		case 7:
		case 8:
		case 9:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_05" : "!!LEAGUE_RANK_SHORT_05");
			break;
		case 10:
		case 11:
		case 12:
		case 13:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_06" : "!!LEAGUE_RANK_SHORT_06");
			break;
		case 14:
		case 15:
		case 16:
		case 17:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_07" : "!!LEAGUE_RANK_SHORT_07");
			break;
		case 18:
		case 19:
		case 20:
		case 21:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_08" : "!!LEAGUE_RANK_SHORT_08");
			break;
		case 22:
		case 23:
		case 24:
		case 25:
			key = ((!inShouldUseShortName) ? "!!LEAGUE_RANK_09" : "!!LEAGUE_RANK_SHORT_09");
			break;
		}
		return KFFLocalization.Get(key);
	}

	public string RankNumber(int rank)
	{
		return " (" + rank + ")";
	}
}
