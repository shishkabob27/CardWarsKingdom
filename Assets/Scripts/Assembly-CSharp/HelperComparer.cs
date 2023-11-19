using System.Collections.Generic;

public class HelperComparer : IComparer<HelperItem>
{
	private class SortFunction
	{
		public delegate int CompareFunction(HelperItem a, HelperItem b);

		public CompareFunction Function;

		public bool Reversed;

		public SortFunction(CompareFunction func, bool reversed)
		{
			Function = func;
			Reversed = reversed;
		}
	}

	private List<SortFunction> mSortFunctions = new List<SortFunction>();

	public HelperComparer(List<SortEntry> sortEntries)
	{
		bool flag = false;
		foreach (SortEntry sortEntry in sortEntries)
		{
			SortFunction.CompareFunction compareFunction = null;
			switch (sortEntry.SortType)
			{
			case SortTypeEnum.Level:
				compareFunction = Level;
				break;
			case SortTypeEnum.STR:
				compareFunction = STR;
				break;
			case SortTypeEnum.INT:
				compareFunction = INT;
				break;
			case SortTypeEnum.DEX:
				compareFunction = DEX;
				break;
			case SortTypeEnum.HP:
				compareFunction = HP;
				break;
			case SortTypeEnum.WeightedStats:
				compareFunction = WeightedStats;
				break;
			case SortTypeEnum.TeamCost:
				compareFunction = TeamCost;
				break;
			case SortTypeEnum.Faction:
				compareFunction = Faction;
				break;
			case SortTypeEnum.Rarity:
				compareFunction = Rarity;
				break;
			}
			if (compareFunction != null)
			{
				mSortFunctions.Add(new SortFunction(compareFunction, sortEntry.Reversed));
			}
		}
	}

	public int Compare(HelperItem a, HelperItem b)
	{
		foreach (SortFunction mSortFunction in mSortFunctions)
		{
			int num = mSortFunction.Function(a, b);
			if (num != 0)
			{
				return (!mSortFunction.Reversed) ? num : (-1 * num);
			}
		}
		return 0;
	}

	private int Level(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.Level.CompareTo(a.HelperCreature.Creature.Level);
	}

	private int STR(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.STR.CompareTo(a.HelperCreature.Creature.STR);
	}

	private int INT(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.INT.CompareTo(a.HelperCreature.Creature.INT);
	}

	private int DEX(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.DEX.CompareTo(a.HelperCreature.Creature.DEX);
	}

	private int HP(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.HP.CompareTo(a.HelperCreature.Creature.HP);
	}

	private int WeightedStats(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.GetWeightedStats().CompareTo(a.HelperCreature.Creature.GetWeightedStats());
	}

	private int TeamCost(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.currentTeamCost.CompareTo(a.HelperCreature.Creature.currentTeamCost);
	}

	private int Faction(HelperItem a, HelperItem b)
	{
		return a.HelperCreature.Creature.Form.Faction.CompareTo(b.HelperCreature.Creature.Form.Faction);
	}

	private int Rarity(HelperItem a, HelperItem b)
	{
		return b.HelperCreature.Creature.StarRating.CompareTo(a.HelperCreature.Creature.StarRating);
	}
}
