using System;
using System.Collections.Generic;

public class GachaSlotData : ILoadableData
{
	public string ID { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public DropTypeEnum CurrencyType { get; private set; }

	public int Cost { get; private set; }

	public int CooldownHours { get; private set; }

	public GachaWeightTable Table { get; private set; }

	public CreatureData FeaturedCreature { get; private set; }

	public List<string> RotatingCreatures { get; private set; }

	public int MinRank { get; private set; }

	public int MaxRank { get; private set; }

	public List<StartEndDate> ShowDates { get; private set; }

	public int ChestType { get; private set; }

	public string ChestSprite { get; private set; }

	public List<int> StarWeights { get; private set; }

	public string KeyName { get; private set; }

	public string KeyUITexture { get; private set; }

	public int KeyRarity { get; private set; }

	public void Populate(Dictionary<string, object> dict)
	{
		ID = TFUtils.LoadString(dict, "ID", string.Empty);
		Name = TFUtils.LoadLocalizedString(dict, "Name", string.Empty);
		Description = TFUtils.LoadLocalizedString(dict, "Description", string.Empty);
		CurrencyType = (DropTypeEnum)(int)Enum.Parse(typeof(DropTypeEnum), TFUtils.LoadString(dict, "CurrencyType", DropTypeEnum.None.ToString()));
		Cost = TFUtils.LoadInt(dict, "Cost", 0);
		CooldownHours = TFUtils.LoadInt(dict, "CooldownHours", 0);
		Table = GachaWeightDataManager.Instance.GetData(TFUtils.LoadString(dict, "Table", string.Empty));
		MinRank = TFUtils.LoadInt(dict, "MinRank", 0);
		MaxRank = TFUtils.LoadInt(dict, "MaxRank", 0);
		ChestType = TFUtils.LoadInt(dict, "ChestType", 1);
		ChestSprite = TFUtils.LoadString(dict, "ChestSprite", string.Empty);
		KeyName = TFUtils.LoadLocalizedString(dict, "KeyName", string.Empty);
		KeyUITexture = TFUtils.LoadString(dict, "KeyUITexture", string.Empty);
		KeyRarity = TFUtils.LoadInt(dict, "KeyRarity", 1);
		string text = TFUtils.LoadString(dict, "FeaturedCreature", null);
		if (text != null)
		{
			FeaturedCreature = CreatureDataManager.Instance.GetData(text);
		}
		RotatingCreatures = new List<string>();
		string[] array = TFUtils.LoadString(dict, "RotatingCreatures", string.Empty).Split(',');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string text3 = text2.Trim();
			if (!(text3 == string.Empty))
			{
				RotatingCreatures.Add(text3);
			}
		}
		int num = 1;
		while (true)
		{
			string text4 = TFUtils.LoadString(dict, "StartDate" + num, null);
			string text5 = TFUtils.LoadString(dict, "EndDate" + num, null);
			if (text4 == null || text5 == null)
			{
				break;
			}
			if (ShowDates == null)
			{
				ShowDates = new List<StartEndDate>();
			}
			StartEndDate startEndDate = new StartEndDate();
			startEndDate.StartDate = DateTime.Parse(text4);
			startEndDate.EndDate = DateTime.Parse(text5);
			ShowDates.Add(startEndDate);
			num++;
		}
		StarWeights = new List<int>();
		int num2 = 1;
		while (true)
		{
			int num3 = TFUtils.LoadInt(dict, "Star" + num2 + "Weight", -1);
			if (num3 == -1)
			{
				break;
			}
			StarWeights.Add(num3);
			num2++;
		}
		if (TFUtils.LoadBool(dict, "IsBasePremium", false))
		{
			GachaSlotDataManager.PremiumGachaCost = Cost;
		}
	}

	public bool ConditionsMet()
	{
		int mCurrentLevel = Singleton<PlayerInfoScript>.Instance.RankXpLevelData.mCurrentLevel;
		if (MinRank > 0 && MinRank > mCurrentLevel)
		{
			return false;
		}
		if (MaxRank > 0 && MaxRank < mCurrentLevel)
		{
			return false;
		}
		if (ShowDates != null)
		{
			bool flag = false;
			foreach (StartEndDate showDate in ShowDates)
			{
				if (showDate.IsWithinDates())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}
}
