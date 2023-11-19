public class AreaAtkSpikeOnOppDeploy : OnOppDeploy
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val1);
			}
		}
		return true;
	}
}
