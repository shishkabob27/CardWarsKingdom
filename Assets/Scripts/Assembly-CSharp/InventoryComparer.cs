using System;
using System.Collections.Generic;

public class InventoryComparer : IComparer<InventorySlotItem>
{
	private class SortFunction
	{
		public delegate int CompareFunction(InventorySlotItem a, InventorySlotItem b);

		public CompareFunction Function;

		public bool Reversed;

		public SortFunction(CompareFunction func, bool reversed)
		{
			Function = func;
			Reversed = reversed;
		}
	}

	private List<SortFunction> mFirstPassSortFunctions = new List<SortFunction>();

	private List<SortFunction> mCreatureSortFunctions = new List<SortFunction>();

	private List<SortFunction> mCardSortFunctions = new List<SortFunction>();

	private List<SortFunction> mEvoMaterialSortFunctions = new List<SortFunction>();

	private List<SortFunction> mXPMaterialSortFunctions = new List<SortFunction>();

	private List<InventorySlotType> mTypeOrder = new List<InventorySlotType>();

	public InventoryComparer(InventorySlotType primaryType, List<SortEntry> creatureSorts, List<SortEntry> cardSorts, bool evoableCreaturesFirst)
	{
		switch (primaryType)
		{
		case InventorySlotType.Creature:
			mTypeOrder.Add(InventorySlotType.Creature);
			mTypeOrder.Add(InventorySlotType.Card);
			mTypeOrder.Add(InventorySlotType.XPMaterial);
			mTypeOrder.Add(InventorySlotType.EvoMaterial);
			break;
		case InventorySlotType.Card:
			mTypeOrder.Add(InventorySlotType.Card);
			mTypeOrder.Add(InventorySlotType.Creature);
			mTypeOrder.Add(InventorySlotType.XPMaterial);
			mTypeOrder.Add(InventorySlotType.EvoMaterial);
			break;
		case InventorySlotType.EvoMaterial:
			mTypeOrder.Add(InventorySlotType.Creature);
			mTypeOrder.Add(InventorySlotType.EvoMaterial);
			mTypeOrder.Add(InventorySlotType.XPMaterial);
			mTypeOrder.Add(InventorySlotType.Card);
			break;
		case InventorySlotType.XPMaterial:
			mTypeOrder.Add(InventorySlotType.XPMaterial);
			mTypeOrder.Add(InventorySlotType.Creature);
			mTypeOrder.Add(InventorySlotType.EvoMaterial);
			mTypeOrder.Add(InventorySlotType.Card);
			break;
		}
		mTypeOrder.Add(InventorySlotType.Empty);
		mTypeOrder.Add(InventorySlotType.Purchase);
		mFirstPassSortFunctions.Add(new SortFunction(SlotType, false));
		mFirstPassSortFunctions.Add(new SortFunction(FilterIrrelevantSlots, false));
		if (evoableCreaturesFirst)
		{
			mCreatureSortFunctions.Add(new SortFunction(Evoable, false));
		}
		foreach (SortEntry creatureSort in creatureSorts)
		{
			SortFunction.CompareFunction compareFunction = GetCompareFunction(creatureSort.SortType);
			if (compareFunction != null)
			{
				mCreatureSortFunctions.Add(new SortFunction(compareFunction, creatureSort.Reversed));
			}
		}
		if (creatureSorts.Find((SortEntry m) => m.SortType == SortTypeEnum.Newest) == null)
		{
			mCreatureSortFunctions.Add(new SortFunction(Newest, false));
		}
		foreach (SortEntry cardSort in cardSorts)
		{
			SortFunction.CompareFunction compareFunction2 = GetCompareFunction(cardSort.SortType);
			if (compareFunction2 != null)
			{
				mCardSortFunctions.Add(new SortFunction(compareFunction2, cardSort.Reversed));
			}
		}
		if (cardSorts.Find((SortEntry m) => m.SortType == SortTypeEnum.Newest) == null)
		{
			mCardSortFunctions.Add(new SortFunction(Newest, false));
		}
		mEvoMaterialSortFunctions.Add(new SortFunction(Rarity, false));
		mEvoMaterialSortFunctions.Add(new SortFunction(Faction, false));
		mEvoMaterialSortFunctions.Add(new SortFunction(Alphabetical, false));
		mXPMaterialSortFunctions.Add(new SortFunction(Faction, false));
		mXPMaterialSortFunctions.Add(new SortFunction(Alphabetical, false));
	}

	public int Compare(InventorySlotItem a, InventorySlotItem b)
	{
		foreach (SortFunction mFirstPassSortFunction in mFirstPassSortFunctions)
		{
			int num = mFirstPassSortFunction.Function(a, b);
			if (num != 0)
			{
				return (!mFirstPassSortFunction.Reversed) ? num : (-1 * num);
			}
		}
		List<SortFunction> list = null;
		switch (a.SlotType)
		{
		case InventorySlotType.Creature:
			list = mCreatureSortFunctions;
			break;
		case InventorySlotType.Card:
			list = mCardSortFunctions;
			break;
		case InventorySlotType.EvoMaterial:
			list = mEvoMaterialSortFunctions;
			break;
		case InventorySlotType.XPMaterial:
			list = mXPMaterialSortFunctions;
			break;
		}
		foreach (SortFunction item in list)
		{
			int num2 = item.Function(a, b);
			if (num2 != 0)
			{
				return (!item.Reversed) ? num2 : (-1 * num2);
			}
		}
		return 0;
	}

	private SortFunction.CompareFunction GetCompareFunction(SortTypeEnum sortType)
	{
		switch (sortType)
		{
		case SortTypeEnum.Newest:
			return Newest;
		case SortTypeEnum.Alphabetical:
			return Alphabetical;
		case SortTypeEnum.Level:
			return Level;
		case SortTypeEnum.STR:
			return STR;
		case SortTypeEnum.INT:
			return INT;
		case SortTypeEnum.DEX:
			return DEX;
		case SortTypeEnum.HP:
			return HP;
		case SortTypeEnum.WeightedStats:
			return WeightedStats;
		case SortTypeEnum.TeamCost:
			return TeamCost;
		case SortTypeEnum.Favorites:
			return Favorites;
		case SortTypeEnum.InUse:
			return InUse;
		case SortTypeEnum.Faction:
			return Faction;
		case SortTypeEnum.Rarity:
			return Rarity;
		case SortTypeEnum.CardCost:
			return CardCost;
		default:
			return null;
		}
	}

	private int SlotType(InventorySlotItem a, InventorySlotItem b)
	{
		return mTypeOrder.IndexOf(a.SlotType).CompareTo(mTypeOrder.IndexOf(b.SlotType));
	}

	private int FilterIrrelevantSlots(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Empty)
		{
			return 1;
		}
		return 0;
	}

	private int Alphabetical(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return a.Creature.Form.Name.CompareTo(b.Creature.Form.Name);
		}
		if (a.SlotType == InventorySlotType.Card)
		{
			return a.Card.Form.Name.CompareTo(b.Card.Form.Name);
		}
		if (a.SlotType == InventorySlotType.EvoMaterial)
		{
			return a.EvoMaterial.Name.CompareTo(b.EvoMaterial.Name);
		}
		if (a.SlotType == InventorySlotType.XPMaterial)
		{
			return a.XPMaterial.Name.CompareTo(b.XPMaterial.Name);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Newest(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.UniqueId.CompareTo(a.Creature.UniqueId);
		}
		if (a.SlotType == InventorySlotType.Card)
		{
			return b.Card.UniqueId.CompareTo(a.Card.UniqueId);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Level(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.Level.CompareTo(a.Creature.Level);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int STR(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.STR.CompareTo(a.Creature.STR);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int INT(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.INT.CompareTo(a.Creature.INT);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int DEX(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.DEX.CompareTo(a.Creature.DEX);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int HP(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.HP.CompareTo(a.Creature.HP);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int WeightedStats(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.GetWeightedStats().CompareTo(a.Creature.GetWeightedStats());
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int TeamCost(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.currentTeamCost.CompareTo(a.Creature.currentTeamCost);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Favorites(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.Favorite.CompareTo(a.Creature.Favorite);
		}
		if (a.SlotType == InventorySlotType.Card)
		{
			return b.Card.Favorite.CompareTo(a.Card.Favorite);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int InUse(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(b.Creature).CompareTo(Singleton<PlayerInfoScript>.Instance.IsCreatureInAnyLoadout(a.Creature));
		}
		if (a.SlotType == InventorySlotType.Card)
		{
			return (b.Card.CreatureUID != 0).CompareTo(a.Card.CreatureUID != 0);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Faction(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return a.Creature.Form.Faction.CompareTo(b.Creature.Form.Faction);
		}
		if (a.SlotType == InventorySlotType.Card)
		{
			return a.Card.Form.Faction.CompareTo(b.Card.Form.Faction);
		}
		if (a.SlotType == InventorySlotType.EvoMaterial)
		{
			return a.EvoMaterial.Faction.CompareTo(b.EvoMaterial.Faction);
		}
		if (a.SlotType == InventorySlotType.XPMaterial)
		{
			return a.XPMaterial.Faction.CompareTo(b.XPMaterial.Faction);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Rarity(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return b.Creature.StarRating.CompareTo(a.Creature.StarRating);
		}
		if (a.SlotType == InventorySlotType.EvoMaterial)
		{
			return b.EvoMaterial.Rarity.CompareTo(a.EvoMaterial.Rarity);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int Evoable(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Creature)
		{
			return (b.Creature.Form.EvolvesTo != string.Empty).CompareTo(a.Creature.Form.EvolvesTo != string.Empty);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}

	private int CardCost(InventorySlotItem a, InventorySlotItem b)
	{
		if (a.SlotType == InventorySlotType.Card)
		{
			return ((int)b.Card.Form.Cost).CompareTo(a.Card.Form.Cost);
		}
		throw new Exception("Unexpected slot types passed into sort: " + a.SlotType.ToString() + ", " + b.SlotType);
	}
}
