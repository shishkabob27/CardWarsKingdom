public class AdjacentShieldOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.Shield, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
