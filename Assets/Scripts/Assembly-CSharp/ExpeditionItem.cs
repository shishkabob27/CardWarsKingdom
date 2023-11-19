using System.Collections.Generic;
using UnityEngine;

public class ExpeditionItem
{
	public ExpeditionDifficultyData Difficulty;

	public ExpeditionNameData NameData;

	public int Duration;

	public uint EndTime;

	public CreatureFaction FavoredClass;

	public int CreatureCount;

	public List<int> UsedCreatureUIDs = new List<int>();

	public bool IsComplete;

	public bool InProgress
	{
		get
		{
			return EndTime != 0;
		}
	}

	public List<GeneralReward> RollRewards(float starsUsed)
	{
		int tierFromStars = ExpeditionDifficultyDataManager.Instance.GetTierFromStars((int)starsUsed);
		tierFromStars = Mathf.Min(tierFromStars, Difficulty.Difficulty);
		tierFromStars = Mathf.Max(tierFromStars, 1);
		List<GeneralReward> list = new List<GeneralReward>();
		ExpeditionDifficultyData dataByDifficulty = ExpeditionDifficultyDataManager.Instance.GetDataByDifficulty(tierFromStars);
		foreach (ExpeditionDifficultyData.PotentialReward potentialReward in dataByDifficulty.PotentialRewards)
		{
			if (potentialReward.RewardType == GeneralReward.TypeEnum.Runes && potentialReward.Chances != null)
			{
				foreach (ExpeditionDifficultyData.PotentialReward.ChanceEntry chance in potentialReward.Chances)
				{
					GeneralReward generalReward = new GeneralReward(potentialReward.RewardType);
					if (chance.Amount > 0)
					{
						generalReward.Quantity = chance.Amount;
					}
					else
					{
						generalReward.Quantity = Random.Range(chance.MinAmount, chance.MaxAmount + 1);
					}
					if (generalReward.Quantity > 0)
					{
						if (chance.Rarity > 0 && chance.Rarity < 4)
						{
							generalReward.Rune = EvoMaterialDataManager.Instance.GetDatabaseByRarity(chance.Rarity, true).RandomElement();
						}
						else
						{
							generalReward.Rune = EvoMaterialDataManager.Instance.GetDatabaseByExpeditionDrop().RandomElement();
						}
						if (generalReward.Rune != null)
						{
							list.Add(generalReward);
						}
					}
				}
				continue;
			}
			int num = potentialReward.RollAmount();
			if (num == 0)
			{
				continue;
			}
			GeneralReward generalReward2 = new GeneralReward(potentialReward.RewardType);
			if (potentialReward.RewardType == GeneralReward.TypeEnum.HardCurrency)
			{
				generalReward2.FreeHardCurrencyQuantity = num;
				generalReward2.Quantity = 0;
			}
			else if (potentialReward.RewardType == GeneralReward.TypeEnum.SpeedUp)
			{
				generalReward2.Quantity = 1;
			}
			else
			{
				generalReward2.Quantity = num;
			}
			if (potentialReward.RewardType == GeneralReward.TypeEnum.Runes)
			{
				while (generalReward2.Rune == null || !generalReward2.Rune.AwakenMat)
				{
					generalReward2.Rune = EvoMaterialDataManager.Instance.GetDatabaseByExpeditionDrop().RandomElement();
				}
			}
			else if (potentialReward.RewardType == GeneralReward.TypeEnum.XPMaterials)
			{
				generalReward2.XPMaterial = XPMaterialDataManager.Instance.GetDatabase().RandomElement();
			}
			else if (potentialReward.RewardType == GeneralReward.TypeEnum.SpeedUp)
			{
				generalReward2.SpeedUp = SpeedUpDataManager.Instance.GetDataByTime(num);
			}
			list.Add(generalReward2);
		}
		return list;
	}
}
