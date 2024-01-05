using System;
using System.Collections.Generic;
using System.Globalization;

public class CardConditions
{
	public enum AmountComparison
	{
		LessThan,
		MoreThan,
		Exactly
	}

	public enum BoardSide
	{
		Friendly,
		Enemy,
		Any
	}

	public enum StatusType
	{
		Positive,
		Negative,
		Any
	}

	private static string GetParam(List<string> parameters, int index)
	{
		if (parameters.Count > index)
		{
			return parameters[index];
		}
		throw new Exception("Card Condition parameter " + (index + 1) + " not found");
	}

	public static bool CardsPlayed(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		PlayerType idx = ((boardSide != 0) ? (!data.OwningPlayer) : data.OwningPlayer);
		int mCardsPlayed = data.Board.GetPlayerState(idx).Statistics.mCardsPlayed;
		return comparison.Compare(mCardsPlayed, rhs);
	}

	public static bool HeroCardPlayed(CardConditionData data)
	{
		return data.Board.GetPlayerState(data.OwningPlayer).Statistics.mHeroCardsPlayed > 0;
	}

	public static bool CreaturesOnBoard(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		int num = 0;
		if (boardSide == BoardSide.Friendly || boardSide == BoardSide.Any)
		{
			num += data.Board.GetPlayerState(data.OwningPlayer).GetCreatureCount();
		}
		if (boardSide == BoardSide.Enemy || boardSide == BoardSide.Any)
		{
			num += data.Board.GetPlayerState(!data.OwningPlayer).GetCreatureCount();
		}
		return comparison.Compare(num, rhs);
	}

	public static bool CreaturesLost(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		int num = 0;
		if (boardSide == BoardSide.Friendly || boardSide == BoardSide.Any)
		{
			num += data.Board.GetPlayerState(data.OwningPlayer).GetDeadCreatureCount();
		}
		if (boardSide == BoardSide.Enemy || boardSide == BoardSide.Any)
		{
			num += data.Board.GetPlayerState(!data.OwningPlayer).GetDeadCreatureCount();
		}
		return comparison.Compare(num, rhs);
	}

	public static bool CardsDrawn(CardConditionData data)
	{
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		int mCardsDrawn = data.Board.GetPlayerState(data.OwningPlayer).Statistics.mCardsDrawn;
		return comparison.Compare(mCardsDrawn, rhs);
	}

	public static bool OpponentDrewHeroCard(CardConditionData data)
	{
		return data.Board.GetPlayerState(!data.OwningPlayer).Statistics.mHeroCardsDrawn > 0;
	}

	public static bool HasStatusCount(CardConditionData data)
	{
		StatusType statusType = (StatusType)(int)Enum.Parse(typeof(StatusType), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		CreatureState creature = data.Board.GetCreature(data.TargetPlayer, data.TargetLane);
		int num = 0;
		if (statusType == StatusType.Positive || statusType == StatusType.Any)
		{
			num += creature.GetStatusTypeCount(global::StatusType.Buff);
		}
		if (statusType == StatusType.Negative || statusType == StatusType.Any)
		{
			num += creature.GetStatusTypeCount(global::StatusType.Debuff);
		}
		return comparison.Compare(num, rhs);
	}

	public static bool T2HasStatusCount(CardConditionData data)
	{
		data.SetToT2();
		return HasStatusCount(data);
	}

	public static bool HasStatus(CardConditionData data)
	{
		string statusName = GetParam(data.Parameters, 0);
		CreatureState creature = data.Board.GetCreature(data.TargetPlayer, data.TargetLane);
		if (data.Parameters.Count > 1)
		{
			StatusState statusState = creature.StatusEffects.Find((StatusState m) => m.Data.ID == statusName);
			if (statusState == null)
			{
				return false;
			}
			AmountComparison amountComparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
			float num = Convert.ToSingle(GetParam(data.Parameters, 2), CultureInfo.InvariantCulture);
			return statusState.GetStackedAmount() >= num;
		}
		return creature.StatusEffects.Contains((StatusState m) => m.Data.ID == statusName);
	}

	public static bool T2HasStatus(CardConditionData data)
	{
		data.SetToT2();
		return HasStatus(data);
	}

	public static bool HasLandscape(CardConditionData data)
	{
		CreatureState creature = data.Board.GetCreature(data.TargetPlayer, data.TargetLane);
		return creature.StatusEffects.Contains((StatusState m) => m.Data.IsLandscape);
	}

	public static bool IsClass(CardConditionData data)
	{
		CreatureFaction creatureFaction = (CreatureFaction)(int)Enum.Parse(typeof(CreatureFaction), GetParam(data.Parameters, 0));
		CreatureState creature = data.Board.GetCreature(data.TargetPlayer, data.TargetLane);
		return creature.Data.Form.Faction == creatureFaction;
	}

	public static bool T2IsClass(CardConditionData data)
	{
		data.SetToT2();
		return IsClass(data);
	}

	public static bool CreaturesJustKilled(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		PlayerType idx = ((boardSide != BoardSide.Enemy) ? (!data.OwningPlayer) : data.OwningPlayer);
		int mCreaturesKilledThisTurn = data.Board.GetPlayerState(idx).Statistics.mCreaturesKilledThisTurn;
		return comparison.Compare(mCreaturesKilledThisTurn, rhs);
	}

	public static bool CreaturesJustDeployed(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		PlayerType idx = ((boardSide != 0) ? (!data.OwningPlayer) : data.OwningPlayer);
		int mCreaturesDeployedThisTurn = data.Board.GetPlayerState(idx).Statistics.mCreaturesDeployedThisTurn;
		return comparison.Compare(mCreaturesDeployedThisTurn, rhs);
	}

	public static bool HealthPercent(CardConditionData data)
	{
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 0));
		int num = Convert.ToInt32(GetParam(data.Parameters, 1));
		CreatureState creature = data.Board.GetCreature(data.TargetPlayer, data.TargetLane);
		return comparison.Compare(creature.HPPct * 100f, num);
	}

	public static bool T2HealthPercent(CardConditionData data)
	{
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 0));
		int num = Convert.ToInt32(GetParam(data.Parameters, 1));
		return comparison.Compare(data.TargetHPPercentBeforeAttack * 100f, num);
	}

	public static bool CreaturesInHand(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		PlayerType idx = ((boardSide != 0) ? (!data.OwningPlayer) : data.OwningPlayer);
		int count = data.Board.GetPlayerState(idx).DeploymentList.Count;
		return comparison.Compare(count, rhs);
	}

	public static bool CardsInHand(CardConditionData data)
	{
		BoardSide boardSide = (BoardSide)(int)Enum.Parse(typeof(BoardSide), GetParam(data.Parameters, 0));
		AmountComparison comparison = (AmountComparison)(int)Enum.Parse(typeof(AmountComparison), GetParam(data.Parameters, 1));
		int rhs = Convert.ToInt32(GetParam(data.Parameters, 2));
		PlayerType idx = ((boardSide != 0) ? (!data.OwningPlayer) : data.OwningPlayer);
		int count = data.Board.GetPlayerState(idx).Hand.Count;
		return comparison.Compare(count, rhs);
	}
}
