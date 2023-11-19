public class GrantAreaThornsOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Thorns, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
