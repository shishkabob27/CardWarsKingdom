public class EraseSpikeGrantBoostOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				CancelStatus(lane.Creature, StatusEnum.AttackSpike);
				CancelStatus(lane.Creature, StatusEnum.MagicSpike);
			}
		}
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackBoost, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.MagicBoost, base.Val1);
			}
		}
		return true;
	}
}
