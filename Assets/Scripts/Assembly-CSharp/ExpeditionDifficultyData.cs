using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ExpeditionDifficultyData : ILoadableData
{
	public class PotentialReward
	{
		public class ChanceEntry
		{
			public int Amount;

			public int MinAmount;

			public int MaxAmount;

			public int Rarity;

			public float Chance;
		}

		public GeneralReward.TypeEnum RewardType;

		public List<ChanceEntry> Chances;

		public int MinAmount;

		public int MaxAmount;

		public int RollAmount()
		{
			if (Chances != null && Chances.Count > 0)
			{
				float num;
				for (num = 1f; num == 1f; num = UnityEngine.Random.Range(0f, 1f))
				{
				}
				foreach (ChanceEntry chance in Chances)
				{
					num -= chance.Chance;
					if (num < 0f)
					{
						return chance.Amount;
					}
				}
				return 0;
			}
			return UnityEngine.Random.Range(MinAmount, MaxAmount + 1);
		}
	}

	public const int MaxCreatures = 5;

	private int mTotalCreatureCountWeights;

	public string ID
	{
		get
		{
			return Difficulty.ToString();
		}
	}

	public int Difficulty { get; private set; }

	public float Chance { get; private set; }

	public string Name { get; private set; }

	public int StarRequirement { get; private set; }

	public int[] CreatureCountWeights { get; private set; }

	public List<int> Durations { get; private set; }

	public List<PotentialReward> PotentialRewards { get; private set; }

	public int MinRarity { get; private set; }

	public int MaxRarity { get; private set; }

	public int MinSoftCurrency { get; private set; }

	public int MaxSoftCurrency { get; private set; }

	public int MinHardCurrency { get; private set; }

	public int MaxHardCurrency { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		Difficulty = TFUtils.LoadInt(dict, "Difficulty", 0);
		Chance = TFUtils.LoadFloat(dict, "Chance", 0f);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		StarRequirement = TFUtils.LoadInt(dict, "StarRequirement", 0);
		MinRarity = TFUtils.LoadInt(dict, "MinRarity", 0);
		MaxRarity = TFUtils.LoadInt(dict, "MaxRarity", 0);
		MinSoftCurrency = 0;
		MaxSoftCurrency = 0;
		MinHardCurrency = 0;
		MaxHardCurrency = 0;
		mTotalCreatureCountWeights = 0;
		CreatureCountWeights = new int[5];
		for (int i = 0; i < 5; i++)
		{
			CreatureCountWeights[i] = TFUtils.LoadInt(dict, i + 1 + "CreatureWeight", 0);
			mTotalCreatureCountWeights += CreatureCountWeights[i];
		}
		Durations = new List<int>();
		string text = TFUtils.LoadString(dict, "Durations", string.Empty);
		string[] array = text.Split(',');
		foreach (string text2 in array)
		{
			string text3 = text2.Trim();
			if (text3 == string.Empty)
			{
				continue;
			}
			try
			{
				int num = Convert.ToInt32(text3.Substring(0, text3.Length - 1));
				if (text3.EndsWith("m"))
				{
					num *= 60;
				}
				else if (text3.EndsWith("h"))
				{
					num *= 3600;
				}
				else
				{
					if (!text3.EndsWith("d"))
					{
						throw new Exception();
					}
					num *= 86400;
				}
				Durations.Add(num);
			}
			catch (Exception)
			{
			}
		}
		PotentialRewards = new List<PotentialReward>();
		ReadReward(GeneralReward.TypeEnum.RankXP, TFUtils.LoadString(dict, "RankXP", null));
		ReadReward(GeneralReward.TypeEnum.SoftCurrency, TFUtils.LoadString(dict, "SoftCurrency", null));
		ReadReward(GeneralReward.TypeEnum.HardCurrency, TFUtils.LoadString(dict, "HardCurrency", null));
		ReadReward(GeneralReward.TypeEnum.XPMaterials, TFUtils.LoadString(dict, "XPMaterials", null));
		ReadReward(GeneralReward.TypeEnum.Runes, TFUtils.LoadString(dict, "EnhanceMaterials", null));
		ReadReward(GeneralReward.TypeEnum.SpeedUp, TFUtils.LoadString(dict, "SpeedUps", null));
	}

	private void ReadReward(GeneralReward.TypeEnum rewardType, string rewardString)
	{
		if (rewardString == null)
		{
			return;
		}
		try
		{
			PotentialReward potentialReward = new PotentialReward();
			PotentialRewards.Add(potentialReward);
			potentialReward.RewardType = rewardType;
			if (rewardString.Contains("-") && !rewardString.Contains("@"))
			{
				string[] array = rewardString.Split('-');
				potentialReward.MinAmount = Convert.ToInt32(array[0].Trim());
				potentialReward.MaxAmount = Convert.ToInt32(array[1].Trim());
				switch (rewardType)
				{
				case GeneralReward.TypeEnum.SoftCurrency:
					MinSoftCurrency = potentialReward.MinAmount;
					MaxSoftCurrency = potentialReward.MaxAmount;
					break;
				case GeneralReward.TypeEnum.HardCurrency:
					MinHardCurrency = potentialReward.MinAmount;
					MaxHardCurrency = potentialReward.MaxAmount;
					break;
				}
				return;
			}
			potentialReward.Chances = new List<PotentialReward.ChanceEntry>();
			float num = 0f;
			string[] array2 = rewardString.Split(',');
			foreach (string text in array2)
			{
				PotentialReward.ChanceEntry chanceEntry = new PotentialReward.ChanceEntry();
				potentialReward.Chances.Add(chanceEntry);
				string[] array3 = text.Split('@');
				if (rewardType == GeneralReward.TypeEnum.Runes)
				{
					if (array3.Length > 1)
					{
						chanceEntry.Rarity = Convert.ToInt32(array3[1].Trim());
					}
					array3 = array3[0].Split('-');
					if (array3.Length > 1)
					{
						chanceEntry.MinAmount = Convert.ToInt32(array3[0].Trim());
						chanceEntry.MaxAmount = Convert.ToInt32(array3[1].Trim());
					}
					else if (array3.Length > 0)
					{
						chanceEntry.Amount = Convert.ToInt32(array3[0].Trim());
					}
				}
				else
				{
					chanceEntry.Amount = Convert.ToInt32(array3[0].Trim());
					chanceEntry.Chance = Convert.ToSingle(array3[1].Replace("%", string.Empty).Trim(), CultureInfo.InvariantCulture) / 100f;
				}
				num += chanceEntry.Chance;
			}
			if (!(num > 1f))
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public int RollCreatureCount()
	{
		int num = UnityEngine.Random.Range(0, mTotalCreatureCountWeights);
		for (int i = 0; i < CreatureCountWeights.Length; i++)
		{
			num -= CreatureCountWeights[i];
			if (num < 0)
			{
				return i + 1;
			}
		}
		return 1;
	}

	public int RollDuration()
	{
		return Durations.RandomElement();
	}
}
