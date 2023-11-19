using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MissionManager : DetachedSingleton<MissionManager>
{
	private class MissionSort : IComparer<Mission>
	{
		public int Compare(Mission a, Mission b)
		{
			if (a.Claimed != b.Claimed)
			{
				return a.Claimed.CompareTo(b.Claimed);
			}
			if (a.Completed != b.Completed)
			{
				return b.Completed.CompareTo(a.Completed);
			}
			if (a.ProgressPct != b.ProgressPct)
			{
				return b.ProgressPct.CompareTo(a.ProgressPct);
			}
			return a.Data.Index.CompareTo(b.Data.Index);
		}
	}

	public MissionProgress GlobalProgress;

	public MissionProgress DailyProgress;

	public MissionProgress BattleProgress;

	public List<Mission> GlobalMissions = new List<Mission>();

	public List<Mission> DailyMissions = new List<Mission>();

	public bool ShouldCheckCompletion { get; set; }

	public void Init()
	{
		GlobalProgress = new MissionProgress();
		DailyProgress = new MissionProgress();
		BattleProgress = new MissionProgress();
	}

	private string SerializeMissionList(List<Mission> missionList)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[");
		for (int i = 0; i < missionList.Count; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(missionList[i].Serialize());
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	private void DeserializeMissionList(object[] missions, List<Mission> missionList)
	{
		missionList.Clear();
		foreach (object obj in missions)
		{
			Dictionary<string, object> dict = obj as Dictionary<string, object>;
			Mission mission = Mission.Deserialize(dict);
			if (mission != null)
			{
				missionList.Add(mission);
			}
		}
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append("\"GlobalProgress\":" + GlobalProgress.Serialize() + ",");
		stringBuilder.Append("\"DailyProgress\":" + DailyProgress.Serialize() + ",");
		stringBuilder.Append("\"GlobalMissions\":" + SerializeMissionList(GlobalMissions) + ",");
		stringBuilder.Append("\"DailyMissions\":" + SerializeMissionList(DailyMissions) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public void Deserialize(Dictionary<string, object> dict)
	{
		GlobalProgress = new MissionProgress(dict["GlobalProgress"] as Dictionary<string, object>);
		DailyProgress = new MissionProgress(dict["DailyProgress"] as Dictionary<string, object>);
		BattleProgress = new MissionProgress();
		if (dict.ContainsKey("GlobalMissions"))
		{
			DeserializeMissionList((object[])dict["GlobalMissions"], GlobalMissions);
		}
		if (dict.ContainsKey("DailyMissions"))
		{
			DeserializeMissionList((object[])dict["DailyMissions"], DailyMissions);
		}
	}

	public bool IsTimeForDailyMissions()
	{
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		DateTime nowTime = instance.GetNowTime();
		DateTime dateTime = new DateTime(instance.SaveData.DailyMissionTimestamp.Year, instance.SaveData.DailyMissionTimestamp.Month, instance.SaveData.DailyMissionTimestamp.Day);
		DateTime dateTime2 = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);
		return dateTime != dateTime2;
	}

	public void StampDailyMissions()
	{
		PlayerInfoScript instance = Singleton<PlayerInfoScript>.Instance;
		instance.SaveData.DailyMissionTimestamp = instance.GetNowTime();
	}

	public void AssignGlobalMissions()
	{
		int mCurrentLevel = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		List<MissionData> missions = MissionDataManager.Instance.GetMissions(MissionType.Global);
		missions.RemoveAll((MissionData m) => m.MissionPrerequisite != null && !IsGlobalMissionCompletedAndClaimed(m.MissionPrerequisite));
		MissionData data;
		foreach (MissionData item2 in missions)
		{
			data = item2;
			Mission mission = GlobalMissions.Find((Mission m) => m.Data.ID == data.ID);
			if (mission == null && data.RankRequirement <= mCurrentLevel)
			{
				Mission item = Mission.Instantiate(data);
				GlobalMissions.Add(item);
			}
		}
	}

	private bool IsGlobalMissionCompletedAndClaimed(MissionData mission)
	{
		Mission mission2 = GlobalMissions.Find((Mission m) => m.Data == mission);
		return mission2 != null && mission2.Completed && mission2.Claimed;
	}

	public List<Mission> AssignDailyMissions()
	{
		string[] array = new string[3] { "All_Dailies_1", "All_Dailies_2", "All_Dailies_3" };
		DailyMissions.Clear();
		DailyProgress.Reset();
		int rank = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		int num = UnityEngine.Random.Range(0, array.Length);
		MissionData data = MissionDataManager.Instance.GetData(array[num]);
		Mission item = Mission.Instantiate(data);
		DailyMissions.Add(item);
		for (int i = 0; i < 5; i++)
		{
			if (i == 1 || i == 0)
			{
				continue;
			}
			List<MissionData> missions = MissionDataManager.Instance.GetMissions((MissionType)i);
			missions.RemoveAll((MissionData m) => m.RankRequirement > rank);
			if (i == 4)
			{
				missions.RemoveAll((MissionData m) => m.ID.StartsWith("All_Dailies"));
			}
			if (missions.Count != 0)
			{
				DailyMissions.Add(Mission.Instantiate(missions.RandomElement()));
			}
		}
		return new List<Mission>(DailyMissions);
	}

	public void ResetBattle()
	{
		BattleProgress.Reset();
	}

	public void ProcessWin()
	{
		GlobalProgress.BattlesWon++;
		DailyProgress.BattlesWon++;
		BattleProgress.BattlesWon++;
		if (Singleton<PlayerInfoScript>.Instance.StateData.MultiplayerMode)
		{
			GlobalProgress.PvPBattlesWon++;
			DailyProgress.PvPBattlesWon++;
			BattleProgress.PvPBattlesWon++;
			return;
		}
		if (Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.League.QuestLine == QuestLineEnum.Special)
		{
			GlobalProgress.DungeonBattlesWon++;
			DailyProgress.DungeonBattlesWon++;
			BattleProgress.DungeonBattlesWon++;
			return;
		}
		GlobalProgress.LeagueBattlesWon++;
		DailyProgress.LeagueBattlesWon++;
		BattleProgress.LeagueBattlesWon++;
		if (!Singleton<PlayerInfoScript>.Instance.IsLeagueComplete(Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.League) || Singleton<PlayerInfoScript>.Instance.StateData.CurrentActiveQuest.League.IsFinalMainLineLeague)
		{
			GlobalProgress.HighestLeagueBattlesWon++;
			DailyProgress.HighestLeagueBattlesWon++;
			BattleProgress.HighestLeagueBattlesWon++;
		}
	}

	public void AddStarsEarned(int stars)
	{
		GlobalProgress.StarsEarned += stars;
	}

	public void ProcessMessage(GameMessage Message)
	{
		if (Message.Action == GameEvent.CREATURE_ATTACKED && Message.Creature.Owner.Type == PlayerType.User)
		{
			GlobalProgress.Attacks++;
			DailyProgress.Attacks++;
			BattleProgress.Attacks++;
			if (Message.IsCritical)
			{
				GlobalProgress.Criticals++;
				DailyProgress.Criticals++;
				BattleProgress.Criticals++;
			}
			if (!Message.IsCounter && Message.AttackType == AttackBase.INT)
			{
				GlobalProgress.MagicAttacks++;
				DailyProgress.MagicAttacks++;
				BattleProgress.MagicAttacks++;
			}
			if (Message.IsDrag)
			{
				GlobalProgress.DragAttacks++;
				DailyProgress.DragAttacks++;
				BattleProgress.DragAttacks++;
			}
			else if (!Message.IsCounter)
			{
				GlobalProgress.CardAttacks++;
				DailyProgress.CardAttacks++;
				BattleProgress.CardAttacks++;
			}
		}
		if (Message.Action == GameEvent.GAIN_BUFF && Message.WhichPlayer.Type == PlayerType.User)
		{
			GlobalProgress.BuffsGranted++;
			DailyProgress.BuffsGranted++;
			BattleProgress.BuffsGranted++;
		}
		if (Message.Action == GameEvent.GAIN_DEBUFF && Message.WhichPlayer.Type != PlayerType.User)
		{
			GlobalProgress.StatusEffectsInflicted++;
			DailyProgress.StatusEffectsInflicted++;
			BattleProgress.StatusEffectsInflicted++;
		}
		if (Message.Action == GameEvent.STATUS_STACKED && Message.WhichPlayer.Game.WhoseTurn.Type == PlayerType.User)
		{
			GlobalProgress.EffectsStacked++;
			DailyProgress.EffectsStacked++;
			BattleProgress.EffectsStacked++;
		}
		if (Message.Action == GameEvent.CREATURE_DIED)
		{
			if (Message.Creature.Owner.Type == PlayerType.User)
			{
				GlobalProgress.CreaturesLost++;
				DailyProgress.CreaturesLost++;
				BattleProgress.CreaturesLost++;
			}
			else
			{
				GlobalProgress.CreaturesKilled++;
				DailyProgress.CreaturesKilled++;
				BattleProgress.CreaturesKilled++;
				GlobalProgress.FactionKills[(int)Message.Creature.Data.Faction]++;
				DailyProgress.FactionKills[(int)Message.Creature.Data.Faction]++;
				BattleProgress.FactionKills[(int)Message.Creature.Data.Faction]++;
			}
		}
		if (Message.Action == GameEvent.CARD_PLAYED && Message.WhichPlayer.Type == PlayerType.User)
		{
			GlobalProgress.CardsPlayed++;
			DailyProgress.CardsPlayed++;
			BattleProgress.CardsPlayed++;
			GlobalProgress.FactionCards[(int)Message.Card.Faction]++;
			DailyProgress.FactionCards[(int)Message.Card.Faction]++;
			BattleProgress.FactionCards[(int)Message.Card.Faction]++;
			if (Message.Card.IsLeaderCard)
			{
				GlobalProgress.SuperCardsPlayed++;
				DailyProgress.SuperCardsPlayed++;
				BattleProgress.SuperCardsPlayed++;
			}
			foreach (StatusData key in Message.Card.StatusValues.Keys)
			{
				if (key.ID == "DirectHealing")
				{
					GlobalProgress.HealCardsPlayed++;
					DailyProgress.HealCardsPlayed++;
					BattleProgress.HealCardsPlayed++;
					break;
				}
			}
		}
		if (Message.Action == GameEvent.CREATURE_DEPLOYED && Message.Creature.Owner.Type == PlayerType.User)
		{
			GlobalProgress.DeployedCreatures++;
			DailyProgress.DeployedCreatures++;
			BattleProgress.DeployedCreatures++;
			GlobalProgress.DeployedFaction[(int)Message.Creature.Data.Faction]++;
			DailyProgress.DeployedFaction[(int)Message.Creature.Data.Faction]++;
			BattleProgress.DeployedFaction[(int)Message.Creature.Data.Faction]++;
		}
		if (Message.Action == GameEvent.SPEND_ACTION_POINTS && Message.WhichPlayer.Type == PlayerType.User)
		{
			GlobalProgress.EnergyUsed += (int)Message.Amount;
			DailyProgress.EnergyUsed += (int)Message.Amount;
			BattleProgress.EnergyUsed += (int)Message.Amount;
		}
	}

	public void CheckCompletion()
	{
		foreach (Mission dailyMission in DailyMissions)
		{
			if (!dailyMission.Completed && dailyMission.IsComplete())
			{
				GlobalProgress.DailiesCompleted++;
				DailyProgress.DailiesCompleted++;
				BattleProgress.DailiesCompleted++;
				dailyMission.Completed = true;
			}
		}
		foreach (Mission globalMission in GlobalMissions)
		{
			if (!globalMission.Completed && globalMission.IsComplete())
			{
				globalMission.Completed = true;
			}
		}
	}

	public void OnCreaturesFused(List<InventorySlotItem> creatureItems)
	{
		foreach (InventorySlotItem creatureItem in creatureItems)
		{
			GlobalProgress.CreaturesFused++;
			DailyProgress.CreaturesFused++;
			BattleProgress.CreaturesFused++;
			int faction = (int)creatureItem.Creature.Faction;
			GlobalProgress.FactionFused[faction]++;
			DailyProgress.FactionFused[faction]++;
			BattleProgress.FactionFused[faction]++;
		}
		ShouldCheckCompletion = true;
	}

	public void OnCardCreated()
	{
		GlobalProgress.CardsCreated++;
		DailyProgress.CardsCreated++;
		BattleProgress.CardsCreated++;
		ShouldCheckCompletion = true;
	}

	public void OnCardsSold(int amount)
	{
		GlobalProgress.CardsSold += amount;
		DailyProgress.CardsSold += amount;
		BattleProgress.CardsSold += amount;
		ShouldCheckCompletion = true;
	}

	public void OnCardsCollected(int amount)
	{
	}

	public void OnCreaturesCollected(int amount)
	{
		GlobalProgress.CreaturesCollected += amount;
		DailyProgress.CreaturesCollected += amount;
		BattleProgress.CreaturesCollected += amount;
	}

	public void OnCreaturesSold(int amount)
	{
		GlobalProgress.CreaturesSold += amount;
		DailyProgress.CreaturesSold += amount;
		BattleProgress.CreaturesSold += amount;
		ShouldCheckCompletion = true;
	}

	public void OnRunesCollected(int amount)
	{
		GlobalProgress.RunesCollected += amount;
		DailyProgress.RunesCollected += amount;
		BattleProgress.RunesCollected += amount;
	}

	public void OnRunesSold(int amount)
	{
		GlobalProgress.RunesSold += amount;
		DailyProgress.RunesSold += amount;
		BattleProgress.RunesSold += amount;
		ShouldCheckCompletion = true;
	}

	public void OnXPMaterialsCollected(int amount)
	{
		GlobalProgress.XPMaterialsCollected += amount;
		DailyProgress.XPMaterialsCollected += amount;
		BattleProgress.XPMaterialsCollected += amount;
	}

	public void OnCoinsCollected(int amount)
	{
		GlobalProgress.CoinsCollected += amount;
		DailyProgress.CoinsCollected += amount;
		BattleProgress.CoinsCollected += amount;
	}

	public void OnCoinsSpent(int amount)
	{
		GlobalProgress.CoinsSpent += amount;
		DailyProgress.CoinsSpent += amount;
		BattleProgress.CoinsSpent += amount;
		ShouldCheckCompletion = true;
	}

	public void OnTeethCollected(int amount)
	{
		GlobalProgress.TeethCollected += amount;
		DailyProgress.TeethCollected += amount;
		BattleProgress.TeethCollected += amount;
	}

	public void OnTeethSpent(int amount)
	{
		GlobalProgress.TeethSpent += amount;
		DailyProgress.TeethSpent += amount;
		BattleProgress.TeethSpent += amount;
		ShouldCheckCompletion = true;
	}

	public void OnUseStamina(int amount)
	{
		GlobalProgress.StaminaUsed += amount;
		DailyProgress.StaminaUsed += amount;
		BattleProgress.StaminaUsed += amount;
		ShouldCheckCompletion = true;
	}

	public int GetCompletedMissionCount()
	{
		int num = 0;
		foreach (Mission globalMission in GlobalMissions)
		{
			if (globalMission.Completed && !globalMission.Claimed)
			{
				num++;
			}
		}
		foreach (Mission dailyMission in DailyMissions)
		{
			if (dailyMission.Completed)
			{
				num++;
			}
		}
		return num;
	}

	public void RemoveDailyMission(Mission mission)
	{
		if (DailyMissions.Remove(mission))
		{
		}
	}

	public float GetMinutesUntilQuestRefresh()
	{
		DateTime dateTime = Singleton<PlayerInfoScript>.Instance.SaveData.DailyMissionTimestamp - Singleton<PlayerInfoScript>.Instance.SaveData.DailyMissionTimestamp.TimeOfDay + new TimeSpan(1, 0, 0, 0);
		return (float)(dateTime - Singleton<PlayerInfoScript>.Instance.GetNowTime()).TotalMinutes;
	}

	public void SortMissions()
	{
		MissionSort comparer = new MissionSort();
		DailyMissions.Sort(comparer);
		GlobalMissions.Sort(comparer);
	}
}
