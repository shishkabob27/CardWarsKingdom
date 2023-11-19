using System.Collections.Generic;

public class RandomDungeonFloorData : ILoadableData
{
	public class DropTableEntry
	{
		public DropTypeEnum DropType;

		public int Weight;

		public int TargetedWeight;

		public int Quantity;

		public int TargetedQuantity;

		public int Rarity;
	}

	private List<DropTableEntry> mDropTable;

	public string ID { get; private set; }

	public int AvgCreatureLevel { get; private set; }

	public int CreatureLevelVar { get; private set; }

	public int MinRarity { get; private set; }

	public int MaxRarity { get; private set; }

	public float AvgPassivePercent { get; private set; }

	public float PassivePercentVar { get; private set; }

	public int EnemyCount { get; private set; }

	public int Battles { get; private set; }

	public int Paths { get; private set; }

	public int RankXPReward { get; private set; }

	public int TargetedRankXPReward { get; private set; }

	public string TargetedCreatureTable { get; private set; }

	public int TargetedCreatureLevel { get; private set; }

	public int DisplayCreatureRarity { get; private set; }

	public float TargetedHardCurrencyChance { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "Floor", string.Empty);
		AvgCreatureLevel = TFUtils.LoadInt(dict, "AvgCreatureLevel", 0);
		CreatureLevelVar = TFUtils.LoadInt(dict, "CreatureLevelVar", 0);
		MinRarity = TFUtils.LoadInt(dict, "MinRarity", 0);
		MaxRarity = TFUtils.LoadInt(dict, "MaxRarity", 0);
		AvgPassivePercent = TFUtils.LoadFloat(dict, "AvgPassivePercent", 0f);
		PassivePercentVar = TFUtils.LoadFloat(dict, "PassivePercentVar", 0f);
		EnemyCount = TFUtils.LoadInt(dict, "EnemyCount", 0);
		Battles = TFUtils.LoadInt(dict, "Battles", 0);
		Paths = TFUtils.LoadInt(dict, "Paths", 0);
		RankXPReward = TFUtils.LoadInt(dict, "RankXP", 0);
		TargetedRankXPReward = TFUtils.LoadInt(dict, "TargetedRankXP", 0);
		TargetedCreatureTable = TFUtils.LoadString(dict, "TargetedCreatureTable", string.Empty);
		TargetedCreatureLevel = TFUtils.LoadInt(dict, "TargetedCreatureLevel", 0);
		DisplayCreatureRarity = TFUtils.LoadInt(dict, "DisplayCreatureRarity", 0);
		TargetedHardCurrencyChance = TFUtils.LoadFloat(dict, "TargetedHardCurrencyChance", 0f);
		mDropTable = new List<DropTableEntry>();
		DropTableEntry dropTableEntry = new DropTableEntry();
		dropTableEntry.DropType = DropTypeEnum.SoftCurrency;
		dropTableEntry.Quantity = TFUtils.LoadInt(dict, "SoftCurrencyAmount", 0);
		dropTableEntry.TargetedQuantity = TFUtils.LoadInt(dict, "TargetedSoftCurrencyAmount", 0);
		dropTableEntry.Weight = (dropTableEntry.TargetedWeight = TFUtils.LoadInt(dict, "SoftCurrencyWeight", 0));
		mDropTable.Add(dropTableEntry);
		dropTableEntry = new DropTableEntry();
		dropTableEntry.DropType = DropTypeEnum.SocialCurrency;
		dropTableEntry.Quantity = (dropTableEntry.TargetedQuantity = TFUtils.LoadInt(dict, "SocialCurrencyAmount", 0));
		dropTableEntry.Weight = TFUtils.LoadInt(dict, "SocialCurrencyWeight", 0);
		dropTableEntry.TargetedWeight = TFUtils.LoadInt(dict, "TargetedSocialCurrencyWeight", 0);
		mDropTable.Add(dropTableEntry);
		for (int i = 1; i <= 5; i++)
		{
			string text = "Material" + i + "StarWeight";
			int num = TFUtils.LoadInt(dict, text, 0);
			if (num != 0)
			{
				dropTableEntry = new DropTableEntry();
				dropTableEntry.DropType = DropTypeEnum.EvoMaterial;
				dropTableEntry.Rarity = i;
				dropTableEntry.Quantity = (dropTableEntry.TargetedQuantity = 1);
				dropTableEntry.Weight = num;
				dropTableEntry.TargetedWeight = TFUtils.LoadInt(dict, "Targeted" + text, 0);
				mDropTable.Add(dropTableEntry);
			}
		}
		dropTableEntry = new DropTableEntry();
		dropTableEntry.DropType = DropTypeEnum.Creature;
		dropTableEntry.Quantity = (dropTableEntry.TargetedQuantity = 1);
		dropTableEntry.TargetedWeight = TFUtils.LoadInt(dict, "TargetedCreatureWeight", 0);
		mDropTable.Add(dropTableEntry);
	}

	public List<DropTableEntry> BuildDropTable(RandomDungeonRewardData targetedReward)
	{
		List<DropTableEntry> list = new List<DropTableEntry>(mDropTable);
		if (targetedReward.RewardType != DropTypeEnum.Creature)
		{
			list.RemoveAll((DropTableEntry m) => m.DropType == DropTypeEnum.Creature);
		}
		foreach (DropTableEntry item in list)
		{
			if (targetedReward.RewardType == item.DropType)
			{
				item.Quantity = item.TargetedQuantity;
				item.Weight = item.TargetedWeight;
			}
		}
		return list;
	}
}
