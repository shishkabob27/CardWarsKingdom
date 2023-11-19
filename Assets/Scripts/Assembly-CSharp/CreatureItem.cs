using System;
using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class CreatureItem
{
	private static int mMaxUniqueId;

	private CreatureData _Form;

	private int _UniqueId;

	public PassiveSkillAbilitySource PassiveSkillAbility;

	private ObscuredInt _PassiveSkillLevel = 1;

	private ObscuredInt _ExCardSlotsUnlocked = 1;

	public InventorySlotItem[] ExCards = new InventorySlotItem[3];

	public CreatureData Form
	{
		get
		{
			return _Form;
		}
	}

	public int UniqueId
	{
		get
		{
			return _UniqueId;
		}
	}

	public CreatureFaction Faction
	{
		get
		{
			return _Form.Faction;
		}
	}

	public ObscuredInt PassiveFeeds { get; set; }

	public int PassiveSkillLevel
	{
		get
		{
			if (QuestLoadoutEntryData != null)
			{
				return _PassiveSkillLevel;
			}
			if (Form.PassiveData == null)
			{
				return 1;
			}
			return Math.Min(_PassiveSkillLevel, Form.PassiveData.MaxLevel);
		}
		set
		{
			if (Form != null && Form.PassiveData != null)
			{
				if (QuestLoadoutEntryData != null)
				{
					_PassiveSkillLevel = value;
				}
				else
				{
					_PassiveSkillLevel = Math.Min(value, Form.PassiveData.MaxLevel);
				}
			}
			else
			{
				_PassiveSkillLevel = 1;
			}
		}
	}

	public ObscuredInt Xp { get; set; }

	public int MaxLevel
	{
		get
		{
			return CreatureStarRatingDataManager.Instance.GetDataByRating(StarRating).MaxLevel;
		}
	}

	public int TotalMaxLevel
	{
		get
		{
			return Form.TotalMaxLevel;
		}
	}

	public int Level
	{
		get
		{
			if (QuestLoadoutEntryData != null)
			{
				return XPTable.GetCurrentLevel(Xp, int.MaxValue);
			}
			return XPTable.GetCurrentLevel(Xp, MaxLevel);
		}
	}

	private float LevelPct
	{
		get
		{
			return (TotalMaxLevel > 1) ? ((float)(Level - 1) / (float)(TotalMaxLevel - 1)) : 1f;
		}
	}

	private float ExpCurve
	{
		get
		{
			return (TotalMaxLevel > 1) ? ((float)(Math.Pow(2.0, LevelPct) - 1.0)) : 1f;
		}
	}

	private float LevelGrowth
	{
		get
		{
			return ExpCurve + (LevelPct - ExpCurve) * Form.GrowthFactor;
		}
	}

	public int currentTeamCost
	{
		get
		{
			return Form.TeamCost + Level;
		}
	}

	public XPTableData XPTable
	{
		get
		{
			return StarRatingData.XPTable;
		}
	}

	public CreatureStarRatingData StarRatingData
	{
		get
		{
			return CreatureStarRatingDataManager.Instance.GetDataByRating(StarRating);
		}
	}

	public bool Favorite { get; set; }

	public bool IsCollectionDummy { get; set; }

	public int ExCardSlotsUnlocked
	{
		get
		{
			return Form.CardSlots;
		}
	}

	public bool FromOtherPlayer { get; set; }

	public int StarRating { get; set; }

	public int HP
	{
		get
		{
			return (int)Form.MinHP + (int)((float)((int)Form.MaxHP - (int)Form.MinHP) * LevelGrowth);
		}
	}

	public int STR
	{
		get
		{
			return (int)Form.MinSTR + (int)((float)((int)Form.MaxSTR - (int)Form.MinSTR) * LevelGrowth);
		}
	}

	public int DEF
	{
		get
		{
			return Mathf.Min((int)Form.MinDEF + (int)((float)((int)Form.MaxDEF - (int)Form.MinDEF) * LevelGrowth), Form.MaxDEF);
		}
	}

	public int INT
	{
		get
		{
			return (int)Form.MinINT + (int)((float)((int)Form.MaxINT - (int)Form.MinINT) * LevelGrowth);
		}
	}

	public int RES
	{
		get
		{
			return Mathf.Min((int)Form.MinRES + (int)((float)((int)Form.MaxRES - (int)Form.MinRES) * LevelGrowth), Form.MaxRES);
		}
	}

	public int DEX
	{
		get
		{
			return Mathf.Min((int)Form.MinDEX + (int)((float)((int)Form.MaxDEX - (int)Form.MinDEX) * LevelGrowth), Form.MaxDEX);
		}
	}

	public int DeployCost
	{
		get
		{
			int num = Form.DeployCost;
			if (QuestLoadoutEntryData != null)
			{
				num -= QuestLoadoutEntryData.DeployCostFactor;
			}
			return num;
		}
	}

	public IList<CardData> ActionCards
	{
		get
		{
			return _Form.ActionCards;
		}
	}

	public QuestLoadoutEntry QuestLoadoutEntryData { get; set; }

	public bool GivenForEvoTutorial { get; set; }

	public bool EnemyLoadoutCreature { get; set; }

	public CreatureItem(CreatureData Data)
	{
		_Form = Data;
		StarRating = 1;
	}

	public CreatureItem(string CreatureID)
	{
		_Form = CreatureDataManager.Instance.GetData(CreatureID);
		StarRating = 1;
	}

	public CreatureItem(Dictionary<string, object> dict, bool fromOtherPlayer = false)
	{
		FromOtherPlayer = fromOtherPlayer;
		Deserialize(dict, fromOtherPlayer);
	}

	public static void ResetMaxUniqueId()
	{
		mMaxUniqueId = 0;
	}

	public void AssignUniqueId()
	{
		mMaxUniqueId++;
		_UniqueId = mMaxUniqueId;
	}

	public XPLevelData GetLevelData()
	{
		return XPTable.GetLevelData(Xp, MaxLevel);
	}

	public XPLevelData GetLevelDataAt(int currentXp)
	{
		return XPTable.GetLevelData(currentXp, MaxLevel);
	}

	public int GetStat(CreatureStat stat)
	{
		switch (stat)
		{
		case CreatureStat.STR:
			return STR;
		case CreatureStat.INT:
			return INT;
		case CreatureStat.DEX:
			return DEX;
		case CreatureStat.DEF:
			return DEF;
		case CreatureStat.RES:
			return RES;
		case CreatureStat.HP:
			return HP;
		default:
			return 0;
		}
	}

	public int ExCardCount()
	{
		int num = 0;
		InventorySlotItem[] exCards = ExCards;
		foreach (InventorySlotItem inventorySlotItem in exCards)
		{
			if (inventorySlotItem != null)
			{
				num++;
			}
		}
		return num;
	}

	public void SetupPassiveAbilitySources()
	{
		if (Form.PassiveData != null)
		{
			PassiveSkillAbility = new PassiveSkillAbilitySource(Form.PassiveData, PassiveSkillLevel);
		}
		else
		{
			PassiveSkillAbility = null;
		}
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("_T", "CR") + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("ID", Form.ID) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("UniqueID", _UniqueId) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Xp", Xp) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Favorite", Favorite) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Passive", PassiveSkillLevel) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("PassiveFeeds", PassiveFeeds) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("StarRating", StarRating) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private void Deserialize(Dictionary<string, object> dict, bool fromOtherPlayer)
	{
		string iD = TFUtils.LoadString(dict, "ID", string.Empty);
		_Form = CreatureDataManager.Instance.GetData(iD);
		if (_Form == null)
		{
		}
		_UniqueId = TFUtils.LoadInt(dict, "UniqueID", 0);
		Xp = TFUtils.LoadInt(dict, "Xp", 0);
		Favorite = TFUtils.LoadBool(dict, "Favorite", false);
		PassiveSkillLevel = TFUtils.LoadInt(dict, "Passive", 1);
		PassiveFeeds = TFUtils.LoadInt(dict, "PassiveFeeds", 0);
		StarRating = TFUtils.LoadInt(dict, "StarRating", 1);
		if (!fromOtherPlayer && _UniqueId > mMaxUniqueId)
		{
			mMaxUniqueId = _UniqueId;
		}
	}

	public void CalculateXpFusion(List<CreatureItem> fodderList, List<XPMaterialData> xpMaterials, out int xpGranted, out int cost)
	{
		xpGranted = 0;
		cost = 0;
		float num = 1.5f;
		if (Singleton<TutorialController>.Instance.IsBlockActive("XpFusion"))
		{
			num = 1.3f;
		}
		foreach (CreatureItem fodder in fodderList)
		{
			int num2 = fodder.Form.MinFeedXp + (int)((float)(fodder.Form.MaxFeedXp - fodder.Form.MinFeedXp) * fodder.LevelPct);
			if (SameClassBonusXp(fodder))
			{
				num2 = (int)((float)num2 * num);
			}
			xpGranted += num2;
			cost += num2;
		}
		foreach (XPMaterialData xpMaterial in xpMaterials)
		{
			int num3 = (int)((float)xpMaterial.FeedXP * ((!SameClassBonusXp(xpMaterial)) ? 1f : num));
			xpGranted += num3;
			cost += num3;
		}
	}

	public bool SameClassBonusXp(CreatureItem creature)
	{
		return creature.Form.Faction == Form.Faction;
	}

	public bool SameClassBonusXp(XPMaterialData xpMaterial)
	{
		return xpMaterial.Faction == Form.Faction;
	}

	public int GetSellPrice()
	{
		return Form.SellPrice;
	}

	public void Evolve()
	{
		CreatureData data = CreatureDataManager.Instance.GetData(Form.EvolvesTo);
		if (data != null)
		{
			_Form = data;
		}
	}

	public void EnhanceStarRating()
	{
		int level = Level;
		StarRating++;
		Xp = XPTable.GetXpToReachLevel(level);
	}

	public void RemoveExCards()
	{
		for (int i = 0; i < ExCards.Length; i++)
		{
			if (ExCards[i] != null)
			{
				ExCards[i].Card.CreatureUID = 0;
				ExCards[i].Card.CreatureSlot = 0;
				ExCards[i] = null;
			}
		}
	}

	public string PassiveNameString()
	{
		if (!Form.HasPassiveAbility())
		{
			return string.Empty;
		}
		return Form.PassiveData.DisplayName;
	}

	public string PassiveLevelString(bool addFeedStatus = false)
	{
		if (!Form.HasPassiveAbility())
		{
			return string.Empty;
		}
		bool flag = PassiveSkillLevel >= Form.PassiveData.MaxLevel;
		string newValue = PassiveSkillLevel.ToString();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(KFFLocalization.Get("!!LEVEL_X").Replace("<val1>", newValue));
		if (addFeedStatus)
		{
			if (flag)
			{
				stringBuilder.Append("  (MAX)");
			}
			else
			{
				stringBuilder.Append("  (" + PassiveFeeds.ToString() + "/" + Form.PassiveData.FeedsPerLevel + " Feeds)");
			}
		}
		stringBuilder.Append(":");
		return stringBuilder.ToString();
	}

	public string DetailsFormatString()
	{
		string details = Form.Details;
		return string.Empty;
	}

	public string PassiveLevelDetailFormatString(int overrideLevel = -1)
	{
		if (!Form.HasPassiveAbility())
		{
			return string.Empty;
		}
		int num = PassiveSkillLevel;
		if (overrideLevel != -1)
		{
			num = overrideLevel;
		}
		bool flag = num >= Form.PassiveData.MaxLevel;
		string newValue = num.ToString();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ffd700]");
		stringBuilder.Append(KFFLocalization.Get("!!SKILLLEVEL_X").Replace("<val1>", newValue));
		stringBuilder.Append(":");
		return stringBuilder.ToString();
	}

	public string BuildPassiveDescriptionString(bool isBattle, int overrideLevel = -1)
	{
		if (Form == null)
		{
			return string.Empty;
		}
		if (!Form.HasPassiveAbility())
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (isBattle)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(Color.yellow.ToHexString());
			stringBuilder.Append("]");
			stringBuilder.Append(KFFLocalization.Get("!!SKILLLEVEL_X").Replace("<val1>", PassiveSkillLevel.ToString()));
			stringBuilder.Append(": [FFFFFF]");
		}
		int level = PassiveSkillLevel;
		if (overrideLevel != -1)
		{
			level = overrideLevel;
		}
		stringBuilder.Append(Form.PassiveData.BuildDescriptionString(level));
		return stringBuilder.ToString();
	}

	public void GrantPassiveFeeds(int feeds)
	{
		if (Form.PassiveData != null)
		{
			PassiveFeeds = (int)PassiveFeeds + feeds;
			int num = (int)PassiveFeeds / Form.PassiveData.FeedsPerLevel;
			PassiveSkillLevel += num;
			PassiveFeeds = (int)PassiveFeeds - num * Form.PassiveData.FeedsPerLevel;
		}
	}

	public float GetWeightedStats()
	{
		float num = (float)DEX / 100f;
		float num2 = (float)MiscParams.CriticalPercent / 100f;
		return (float)HP + (float)(3 * Math.Max(STR, INT)) * (1f + num2 * num) + (float)(5 * (DEF + RES));
	}

	public bool CanEverEvo()
	{
		if (Form.EvolvesTo == string.Empty)
		{
			return false;
		}
		if (Form.AwakenMaterial == null)
		{
			return false;
		}
		return true;
	}

	public bool CanCurrentlyEvo()
	{
		if (!CanEverEvo())
		{
			return false;
		}
		List<InventorySlotItem> foundEvoMats = new List<InventorySlotItem>();
		for (int i = 0; i < 5; i++)
		{
			InventorySlotItem inventorySlotItem = Singleton<PlayerInfoScript>.Instance.SaveData.FindEvoMaterial(delegate(InventorySlotItem slot)
			{
				if (slot.EvoMaterial != Form.AwakenMaterial)
				{
					return false;
				}
				return (!foundEvoMats.Contains((InventorySlotItem m) => m == slot)) ? true : false;
			});
			if (inventorySlotItem != null)
			{
				foundEvoMats.Add(inventorySlotItem);
				continue;
			}
			return false;
		}
		return true;
	}

	public bool IsMaxStarRating()
	{
		return StarRating >= CreatureStarRatingDataManager.Instance.MaxStarRating();
	}

	public int GetEmptyCardSlot()
	{
		for (int i = 0; i < 3; i++)
		{
			if (i < ExCardSlotsUnlocked && ExCards[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public bool AlreadyHasCard(CardData card)
	{
		if (ActionCards.Contains(card))
		{
			return true;
		}
		if (ExCards.Find((InventorySlotItem m) => m != null && m.Card.Form == card) != null)
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		if (Form != null)
		{
			return Form.ToString();
		}
		return "null form";
	}

	public void SetStarRatingFromGacha(int stars)
	{
		StarRating = stars;
		if (stars > 1)
		{
			CreatureStarRatingData dataByRating = CreatureStarRatingDataManager.Instance.GetDataByRating(stars - 1);
			Xp = XPTable.GetXpToReachLevel(dataByRating.MaxLevel);
		}
		else
		{
			Xp = 0;
		}
	}

	public void SetStarRatingToMatchCurrentXP()
	{
		StarRating = 1;
		while (Level > MaxLevel)
		{
			StarRating++;
		}
	}
}
