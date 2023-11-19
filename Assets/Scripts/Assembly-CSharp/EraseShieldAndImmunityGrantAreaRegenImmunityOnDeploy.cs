public class EraseShieldAndImmunityGrantAreaRegenImmunityOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				CancelStatus(lane.Creature, StatusEnum.Shield);
				CancelStatus(lane.Creature, StatusEnum.Immunity);
			}
		}
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Regen, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Immunity, base.Val1);
			}
		}
		return true;
	}
}
