using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : ILoadableData
{
	public const int MAX_QUEST_STARS = 3;

	public int RandomBattleIndex = -1;

	public bool IsFinalRandomBattle;

	public RandomDungeonRewardData RandomDungeonReward;

	private static LeagueData mLastLeague;

	public string ID { get; private set; }

	public string LevelName { get; private set; }

	public string LevelPrefab { get; private set; }

	public string BoardPrefab { get; private set; }

	public LeaderData Opponent { get; private set; }

	public int XPReward { get; private set; }

	public int StaminaCost { get; private set; }

	public string BGM { get; private set; }

	public LeagueData League { get; private set; }

	public string VSIcon { get; private set; }

	public string PortraitBg { get; private set; }

	public int EnemyCount { get; private set; }

	public QuestLoadoutData SetLoadout { get; private set; }

	public QuestLoadoutData RandomLoadout { get; private set; }

	public bool OneTime { get; private set; }

	public CustomAIData CustomAI { get; private set; }

	public bool BroadcastWin { get; private set; }

	public CreatureData RequiredCreature { get; private set; }

	public int ForceFirstPlayer { get; private set; }

	public int ReducedStaminaCost
	{
		get
		{
			return StaminaCost / 2 + StaminaCost % 2;
		}
	}

	public static QuestData HighestMainLineQuest { get; private set; }

	public bool IsRandomDungeonBattle
	{
		get
		{
			return RandomBattleIndex != -1;
		}
	}

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "QuestID", string.Empty);
		LevelName = TFUtils.LoadLocalizedString(dict, "LevelName", string.Empty);
		LevelPrefab = TFUtils.LoadString(dict, "LevelPrefab", string.Empty);
		BoardPrefab = TFUtils.LoadString(dict, "BoardPrefab", string.Empty);
		Opponent = LeaderDataManager.Instance.GetData(TFUtils.LoadString(dict, "Opponent", string.Empty));
		XPReward = TFUtils.LoadInt(dict, "XP", 0);
		StaminaCost = TFUtils.LoadInt(dict, "Stamina", 1);
		BGM = TFUtils.LoadString(dict, "BGM", string.Empty);
		VSIcon = TFUtils.LoadString(dict, "VSIcon", string.Empty);
		PortraitBg = TFUtils.LoadString(dict, "PortraitBg", string.Empty);
		EnemyCount = TFUtils.LoadInt(dict, "EnemyCount", -1);
		OneTime = TFUtils.LoadBool(dict, "OneTime", false);
		BroadcastWin = TFUtils.LoadBool(dict, "BroadcastWin", false);
		string text = TFUtils.LoadString(dict, "ForceFirstPlayer", null);
		if (text == "Player")
		{
			ForceFirstPlayer = 0;
		}
		else if (text == "Opponent")
		{
			ForceFirstPlayer = 1;
		}
		else
		{
			ForceFirstPlayer = -1;
		}
		string text2 = TFUtils.LoadString(dict, "CustomAI", null);
		if (text2 != null)
		{
			CustomAI = CustomAIDataManager.Instance.GetData(text2);
		}
		string text3 = TFUtils.LoadString(dict, "SetLoadout", null);
		if (text3 != null)
		{
			SetLoadout = QuestLoadoutDataManager.Instance.GetData(text3);
		}
		string text4 = TFUtils.LoadString(dict, "RandomLoadout", null);
		if (text4 != null)
		{
			RandomLoadout = QuestLoadoutDataManager.Instance.GetData(text4);
		}
		string text5 = TFUtils.LoadString(dict, "League", string.Empty);
		if (text5 == "PVP")
		{
			mLastLeague = null;
		}
		else if (text5 != string.Empty)
		{
			mLastLeague = LeagueDataManager.Instance.GetData(text5);
		}
		League = mLastLeague;
		if (League != null)
		{
			League.Quests.Add(this);
			if (League.QuestLine == QuestLineEnum.Main)
			{
				HighestMainLineQuest = this;
			}
		}
		string text6 = TFUtils.LoadString(dict, "RequiredCreature", null);
		if (text6 != null)
		{
			RequiredCreature = CreatureDataManager.Instance.GetData(text6);
		}
	}

	public int GetIntQuestId()
	{
		//Discarded unreachable code: IL_0011, IL_001e
		try
		{
			return Convert.ToInt32(ID);
		}
		catch
		{
			return -1;
		}
	}

	public QuestLoadoutData BuildQuestLoadout()
	{
		int i = 0;
		QuestLoadoutData questLoadoutData = new QuestLoadoutData();
		if (SetLoadout != null)
		{
			foreach (QuestLoadoutEntry entry in SetLoadout.Entries)
			{
				questLoadoutData.AddEntry(entry);
				i++;
			}
		}
		if (RandomLoadout != null && EnemyCount != -1)
		{
			if (i >= EnemyCount)
			{
			}
			for (; i < EnemyCount; i++)
			{
				questLoadoutData.AddEntry(RandomLoadout.GetRandomEntry());
			}
		}
		return questLoadoutData;
	}

	public InventorySlotItem[] GetPreMatchCreatures()
	{
		InventorySlotItem[] array = new InventorySlotItem[MiscParams.CreaturesOnTeam];
		for (int i = 0; i < MiscParams.CreaturesOnTeam; i++)
		{
			if (i < SetLoadout.Entries.Count)
			{
				array[i] = new InventorySlotItem(SetLoadout.Entries[i].BuildCreatureItem());
				array[i].Creature.EnemyLoadoutCreature = true;
			}
		}
		return array;
	}

	public bool IsBossQuest()
	{
		if (League == null)
		{
			return false;
		}
		if (League.QuestLine != 0)
		{
			return false;
		}
		return this == League.Quests[League.Quests.Count - 1];
	}

	public bool IsCompletedOneTimeQuest()
	{
		return OneTime && Singleton<PlayerInfoScript>.Instance.GetSpecialQuestStatus(ID) == SpecialQuestStatus.Completed;
	}

	public void CreateRandomQuest(int battleIndex, RandomDungeonRewardData reward, int pathIndex = -1)
	{
		RandomBattleIndex = battleIndex;
		RandomDungeonReward = reward;
		League = LeagueDataManager.Instance.GetData("DailyRandom");
		if (pathIndex != -1)
		{
			LevelName = KFFLocalization.Get("!!RANDOM_DUNGEON_PATH_X").Replace("<val1>", (pathIndex + 1).ToString());
		}
		else
		{
			LevelName = KFFLocalization.Get("!!RANDOM_DUNGEON_BATTLE_X").Replace("<val1>", (battleIndex + 1).ToString());
		}
		ID = LevelName;
		StaminaCost = 1;
		LevelPrefab = "Chamber_BG";
		BoardPrefab = "GameBoard_Forest";
		BGM = "Music_Forest";
		VSIcon = "Icon_VS_Forest";
		PortraitBg = "OilRig";
	}

	public void FinalizeRandomQuest()
	{
		RandomDungeonFloorData currentData = RandomDungeonFloorDataManager.Instance.GetCurrentData();
		IsFinalRandomBattle = RandomBattleIndex >= currentData.Battles - 1;
		if (RandomDungeonReward.RewardType == DropTypeEnum.RankXP)
		{
			XPReward = currentData.TargetedRankXPReward;
		}
		else
		{
			XPReward = currentData.RankXPReward;
		}
		List<LeaderData> list = LeaderDataManager.Instance.GetDatabase().FindAll((LeaderData m) => m.Playable);
		Opponent = list.RandomElement();
		CustomAI = null;
		List<RandomDungeonFloorData.DropTableEntry> list2 = currentData.BuildDropTable(RandomDungeonReward);
		int num = 0;
		foreach (RandomDungeonFloorData.DropTableEntry item in list2)
		{
			num += item.Weight;
		}
		bool flag = false;
		if (RandomDungeonReward.RewardType == DropTypeEnum.HardCurrency)
		{
			if (IsFinalRandomBattle)
			{
				flag = true;
			}
			else if (UnityEngine.Random.Range(0f, 1f) < currentData.TargetedHardCurrencyChance)
			{
				flag = true;
			}
		}
		SetLoadout = new QuestLoadoutData();
		EnemyCount = currentData.EnemyCount;
		int num2 = 0;
		int num3 = currentData.MaxRarity - currentData.MinRarity;
		float num4 = (float)currentData.MinRarity + (float)num3 / 2f;
		for (int i = 0; i < EnemyCount; i++)
		{
			int rarity;
			if (EnemyCount > 1)
			{
				float num5 = (float)i / (float)(EnemyCount - 1);
				float num6 = (float)currentData.MinRarity + (float)num3 * num5;
				rarity = ((!(num6 < num4)) ? Mathf.FloorToInt(num6) : Mathf.CeilToInt(num6));
			}
			else
			{
				rarity = currentData.MaxRarity;
			}
			CreatureData randomDungeonCreature = CreatureDataManager.Instance.GetRandomDungeonCreature(rarity);
			CreatureItem creatureItem = new CreatureItem(randomDungeonCreature);
			int num7 = currentData.AvgCreatureLevel;
			if (num2 != 0)
			{
				num7 -= num2;
				num2 = 0;
			}
			else if (i < EnemyCount - 1)
			{
				num2 = UnityEngine.Random.Range(-currentData.CreatureLevelVar, currentData.CreatureLevelVar);
				num7 += num2;
			}
			if (randomDungeonCreature.HasPassiveAbility())
			{
				creatureItem.PassiveSkillLevel = 1;
				int num8 = randomDungeonCreature.PassiveData.MaxLevel - 1;
				if (num8 > 0)
				{
					float value = currentData.AvgPassivePercent + UnityEngine.Random.Range(0f - currentData.PassivePercentVar, currentData.PassivePercentVar);
					value = Mathf.Clamp01(value);
					creatureItem.PassiveSkillLevel += Mathf.RoundToInt((float)num8 * value);
				}
			}
			for (int j = 0; j < 3; j++)
			{
				creatureItem.ExCards[j] = null;
			}
			float creatureScale = 1f;
			DropTypeEnum dropTypeEnum = DropTypeEnum.SoftCurrency;
			QuestLoadoutEntry.DropInfoClass dropInfoClass = new QuestLoadoutEntry.DropInfoClass();
			if (flag)
			{
				dropTypeEnum = DropTypeEnum.HardCurrency;
				flag = false;
			}
			else
			{
				int num9 = UnityEngine.Random.Range(0, num);
				foreach (RandomDungeonFloorData.DropTableEntry item2 in list2)
				{
					num9 -= item2.Weight;
					if (num9 >= 0)
					{
						continue;
					}
					dropTypeEnum = item2.DropType;
					switch (dropTypeEnum)
					{
					case DropTypeEnum.SoftCurrency:
						dropInfoClass.SoftCurrency = item2.Quantity;
						break;
					case DropTypeEnum.SocialCurrency:
						dropInfoClass.SocialCurrency = item2.Quantity;
						break;
					case DropTypeEnum.EvoMaterial:
					{
						string iD2;
						if (RandomDungeonReward.RewardType == DropTypeEnum.EvoMaterial)
						{
							iD2 = string.Format("Evo_Mat_T{0}_{1}", item2.Rarity.ToString(), RandomDungeonReward.Subtype);
						}
						else
						{
							CreatureFaction creatureFaction2 = CreatureFactionExtensions.RandomFaction();
							iD2 = string.Format("Evo_Mat_T{0}_{1}", item2.Rarity.ToString(), creatureFaction2.ToString());
						}
						dropInfoClass.EvoMaterial = EvoMaterialDataManager.Instance.GetData(iD2);
						break;
					}
					case DropTypeEnum.Creature:
						if (RandomDungeonReward.Subtype == "Dieclops")
						{
							CreatureFaction creatureFaction = CreatureFactionExtensions.RandomFaction();
							string iD = "DieClops_Base_" + creatureFaction;
							dropInfoClass.Creature = CreatureDataManager.Instance.GetData(iD);
							dropInfoClass.CreatureLevel = 1;
						}
						else
						{
							GachaWeightTable data = GachaWeightDataManager.Instance.GetData(currentData.TargetedCreatureTable);
							dropInfoClass.Creature = CreatureDataManager.Instance.GetData(data.Spin().DropID);
							dropInfoClass.CreatureLevel = currentData.TargetedCreatureLevel;
						}
						break;
					}
					break;
				}
			}
			QuestLoadoutEntry entry = new QuestLoadoutEntry(creatureItem, num7, dropTypeEnum, dropInfoClass, creatureScale);
			SetLoadout.AddEntry(entry);
		}
	}
}
