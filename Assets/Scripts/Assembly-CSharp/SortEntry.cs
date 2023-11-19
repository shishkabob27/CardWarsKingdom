using System.Collections.Generic;
using System.Text;

public class SortEntry
{
	public SortTypeEnum SortType = SortTypeEnum.Count;

	public bool Reversed;

	public SortEntry(SortTypeEnum sortType, bool reversed = false)
	{
		SortType = sortType;
		Reversed = reversed;
	}

	public SortEntry(Dictionary<string, object> dict)
	{
		Deserialize(dict);
	}

	public string Serialize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Sort", (int)SortType) + ",");
		stringBuilder.Append(PlayerInfoScript.MakeJS("Rv", Reversed) + ",");
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private void Deserialize(Dictionary<string, object> dict)
	{
		SortType = (SortTypeEnum)TFUtils.LoadInt(dict, "Sort", 0);
		Reversed = TFUtils.LoadBool(dict, "Rv", false);
	}

	public string GetName()
	{
		return GetName(SortType);
	}

	public static string GetName(SortTypeEnum sortType)
	{
		switch (sortType)
		{
		case SortTypeEnum.Newest:
			return KFFLocalization.Get("!!NEWEST");
		case SortTypeEnum.Alphabetical:
			return KFFLocalization.Get("!!ALPHABETICAL");
		case SortTypeEnum.Level:
			return KFFLocalization.Get("!!LEVEL");
		case SortTypeEnum.STR:
			return CreatureStat.STR.DisplayName();
		case SortTypeEnum.INT:
			return CreatureStat.INT.DisplayName();
		case SortTypeEnum.DEX:
			return CreatureStat.DEX.DisplayName();
		case SortTypeEnum.HP:
			return CreatureStat.HP.DisplayName();
		case SortTypeEnum.WeightedStats:
			return KFFLocalization.Get("!!WEIGHTED_STATS");
		case SortTypeEnum.TeamCost:
			return KFFLocalization.Get("!!TEAM_WEIGHT");
		case SortTypeEnum.Favorites:
			return KFFLocalization.Get("!!FAVORITES");
		case SortTypeEnum.InUse:
			return KFFLocalization.Get("!!IN_USE");
		case SortTypeEnum.Rarity:
			return KFFLocalization.Get("!!RARITY");
		case SortTypeEnum.Faction:
			return KFFLocalization.Get("!!CLASS");
		case SortTypeEnum.CardCost:
			return KFFLocalization.Get("!!COST");
		default:
			return string.Empty;
		}
	}

	public string GetDirectionLabel()
	{
		return GetDirectionLabel(SortType, Reversed);
	}

	public static string GetDirectionLabel(SortTypeEnum sortType, bool reversed)
	{
		switch (sortType)
		{
		case SortTypeEnum.Alphabetical:
			return reversed ? KFFLocalization.Get("!!SORT_DESCENDING") : KFFLocalization.Get("!!SORT_ASCENDING");
		case SortTypeEnum.Level:
		case SortTypeEnum.STR:
		case SortTypeEnum.INT:
		case SortTypeEnum.DEX:
		case SortTypeEnum.HP:
		case SortTypeEnum.WeightedStats:
		case SortTypeEnum.TeamCost:
		case SortTypeEnum.Rarity:
		case SortTypeEnum.CardCost:
			return reversed ? KFFLocalization.Get("!!SORT_ASCENDING") : KFFLocalization.Get("!!SORT_DESCENDING");
		case SortTypeEnum.Faction:
			return reversed ? KFFLocalization.Get("!!SORT_REVERSED") : KFFLocalization.Get("!!SORT_NORMAL");
		case SortTypeEnum.Newest:
		case SortTypeEnum.Favorites:
		case SortTypeEnum.InUse:
			return reversed ? KFFLocalization.Get("!!SORT_LAST") : KFFLocalization.Get("!!SORT_FIRST");
		default:
			return string.Empty;
		}
	}
}
