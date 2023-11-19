using System;
using System.Collections.Generic;

public class RandomDungeonRewardData : ILoadableData
{
	private static string mLastType = string.Empty;

	public string ID { get; private set; }

	public DropTypeEnum RewardType { get; private set; }

	public string Subtype { get; private set; }

	public int Weight { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = RandomDungeonRewardDataManager.Instance.GetDatabase().Count.ToString();
		string text = TFUtils.LoadString(dict, "RewardType", string.Empty);
		if (text == "^")
		{
			text = mLastType;
		}
		else
		{
			mLastType = text;
		}
		RewardType = (DropTypeEnum)(int)Enum.Parse(typeof(DropTypeEnum), text);
		Subtype = TFUtils.LoadString(dict, "SubType", string.Empty);
		Weight = TFUtils.LoadInt(dict, "Weight", 0);
	}

	public string RewardLabel()
	{
		if (RewardType == DropTypeEnum.RankXP)
		{
			return KFFLocalization.Get("!!RANK_XP");
		}
		if (RewardType == DropTypeEnum.SoftCurrency)
		{
			return KFFLocalization.Get("!!CURRENCY_SOFT");
		}
		if (RewardType == DropTypeEnum.SocialCurrency)
		{
			return KFFLocalization.Get("!!CURRENCY_PVP");
		}
		if (RewardType == DropTypeEnum.HardCurrency)
		{
			return KFFLocalization.Get("!!CURRENCY_HARD");
		}
		if (RewardType == DropTypeEnum.EvoMaterial)
		{
			if (Subtype == "Red")
			{
				return KFFLocalization.Get("!!RED_RUNES");
			}
			if (Subtype == "Green")
			{
				return KFFLocalization.Get("!!GREEN_RUNES");
			}
			if (Subtype == "Blue")
			{
				return KFFLocalization.Get("!!BLUE_RUNES");
			}
			if (Subtype == "Dark")
			{
				return KFFLocalization.Get("!!DARK_RUNES");
			}
			if (Subtype == "Light")
			{
				return KFFLocalization.Get("!!LIGHT_RUNES");
			}
			if (Subtype == "Special")
			{
				return KFFLocalization.Get("!!RAINBOW_RUNES");
			}
		}
		else if (RewardType == DropTypeEnum.Creature)
		{
			if (Subtype == "Dieclops")
			{
				return KFFLocalization.Get("!!XP_CREATURES");
			}
			RandomDungeonFloorData currentData = RandomDungeonFloorDataManager.Instance.GetCurrentData();
			return KFFLocalization.Get("!!X_STAR_CREATURES").Replace("<val1>", currentData.DisplayCreatureRarity.ToString());
		}
		return string.Empty;
	}
}
