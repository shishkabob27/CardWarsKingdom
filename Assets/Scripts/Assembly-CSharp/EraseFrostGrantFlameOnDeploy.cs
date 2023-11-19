public class EraseFrostGrantFlameOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				CancelStatus(lane.Creature, StatusEnum.FrostArmor);
			}
		}
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.FlameArmor, base.Val1);
			}
		}
		return true;
	}
}
