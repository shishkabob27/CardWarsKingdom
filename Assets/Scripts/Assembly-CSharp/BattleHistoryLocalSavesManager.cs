using System;
using System.Collections.Generic;

public class BattleHistoryLocalSavesManager
{
	private string cachedOpponentFlag;

	private static BattleHistoryLocalSavesManager _instance;

	public static BattleHistoryLocalSavesManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BattleHistoryLocalSavesManager();
			}
			return _instance;
		}
	}

	public string CachedOpponentFlag
	{
		get
		{
			return cachedOpponentFlag;
		}
		set
		{
			cachedOpponentFlag = value;
		}
	}

	public BattleHistory SaveNewBattleHistory()
	{
		if (Singleton<PlayerInfoScript>.Instance.PvPData.OpponentLoadout == null)
		{
			return null;
		}
		PvpSeasonData pvpSeasonData = null;
		if (Singleton<PlayerInfoScript>.Instance.PvPData.RankedMode)
		{
			pvpSeasonData = Singleton<PlayerInfoScript>.Instance.SaveData.ActivePvpSeason;
		}
		string opponentName = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentName;
		string empty = string.Empty;
		string opponentCountryCode = Singleton<OnlinePvPManager>.Instance.GetOpponentCountryCode();
		string season = ((pvpSeasonData == null) ? string.Empty : pvpSeasonData.ID);
		Loadout opponentLoadout = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentLoadout;
		List<string> list = new List<string>();
		for (int i = 0; i < opponentLoadout.CreatureSet.Count; i++)
		{
			if (opponentLoadout.CreatureSet != null)
			{
				list.Add(opponentLoadout.CreatureSet[i].Creature.Form.ID);
			}
		}
		int opponentLevel = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentLevel;
		int opponentBestLevel = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentBestLevel;
		int bestLeagueSeasonID = 0;
		uint recordTime = DateTime.UtcNow.UnixTimestamp();
		string opponentID = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentID;
		string opponentFBID = Singleton<PlayerInfoScript>.Instance.PvPData.OpponentFBID;
		string iD = opponentLoadout.Leader.SelectedSkin.ID;
		BattleHistory battleHistory = new BattleHistory(opponentName, empty, opponentCountryCode, season, false, list, opponentLevel, opponentBestLevel, bestLeagueSeasonID, recordTime, opponentID, opponentFBID, iD);
		while (Singleton<PlayerInfoScript>.Instance.SaveData.BattleHistoryList.Count >= MiscParams.MaxMatchesInHistory)
		{
			Singleton<PlayerInfoScript>.Instance.SaveData.BattleHistoryList.RemoveAt(0);
		}
		Singleton<PlayerInfoScript>.Instance.SaveData.BattleHistoryList.Add(battleHistory);
		return battleHistory;
	}

	public void FinalizeHistoryEntry(BattleHistory entry, bool youWon)
	{
		entry.youWon = youWon;
	}

	public List<BattleHistory> getRankedMatches()
	{
		return Singleton<PlayerInfoScript>.Instance.SaveData.BattleHistoryList.FindAll((BattleHistory m) => m.season != string.Empty);
	}

	public List<BattleHistory> getUnrankedMatches()
	{
		return Singleton<PlayerInfoScript>.Instance.SaveData.BattleHistoryList.FindAll((BattleHistory m) => m.season == string.Empty);
	}
}
