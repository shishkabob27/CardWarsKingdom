using System.Collections.Generic;
using System.Reflection;

public class TargetFilters
{
	public static bool IsEnemy(PlayerType WhichPlayer, LaneState Lane)
	{
		if (Lane.Owner.Type != WhichPlayer)
		{
			return true;
		}
		return false;
	}

	public static bool IsFriendly(PlayerType WhichPlayer, LaneState Lane)
	{
		if (Lane.Owner.Type == WhichPlayer)
		{
			return true;
		}
		return false;
	}

	public static bool HasCreature(PlayerType WhichPlayer, LaneState Lane)
	{
		if (Lane.Creature != null)
		{
			return true;
		}
		return false;
	}

	public static bool IsEmpty(PlayerType WhichPlayer, LaneState Lane)
	{
		if (Lane.Creature == null)
		{
			return true;
		}
		return false;
	}

	private static bool IsFaction(LaneState Lane, CreatureFaction Faction)
	{
		if (Lane.Creature != null && Lane.Creature.Data.Faction == Faction)
		{
			return true;
		}
		return false;
	}

	public static bool IsColorless(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Colorless);
	}

	public static bool IsRed(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Red);
	}

	public static bool IsGreen(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Green);
	}

	public static bool IsBlue(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Blue);
	}

	public static bool IsDark(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Dark);
	}

	public static bool IsLight(PlayerType WhichPlayer, LaneState Lane)
	{
		return IsFaction(Lane, CreatureFaction.Light);
	}

	public static bool IsBuffed(PlayerType WhichPlayer, LaneState Lane)
	{
		return Lane.Creature.HasBuff;
	}

	public static bool IsDebuffed(PlayerType WhichPlayer, LaneState Lane)
	{
		return Lane.Creature.HasDebuff;
	}

	public static bool IsHPFull(PlayerType WhichPlayer, LaneState Lane)
	{
		return Lane.Creature.HP == Lane.Creature.MaxHP;
	}

	public static bool IsHPCritical(PlayerType WhichPlayer, LaneState Lane)
	{
		return Lane.Creature.HPPct <= 0.25f;
	}

	public static List<LaneState> FilterLaneList(PlayerType WhichPlayer, List<LaneState> CurrentList, MethodInfo Filter)
	{
		if (Filter != null)
		{
			List<LaneState> list = new List<LaneState>();
			{
				foreach (LaneState Current in CurrentList)
				{
					if ((bool)Filter.Invoke(null, new object[2] { WhichPlayer, Current }))
					{
						list.Add(Current);
					}
				}
				return list;
			}
		}
		return CurrentList;
	}
}
