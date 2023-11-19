public class GrantRevengeIfAdjHPIsLow : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && adjacentLane.Creature.HPPct < base.Val1Pct)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.Revenge, 1f);
				result = true;
			}
		}
		return result;
	}
}
