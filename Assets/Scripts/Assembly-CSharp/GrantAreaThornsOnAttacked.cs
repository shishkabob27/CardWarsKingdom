public class GrantAreaThornsOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null && !thisAndAdjacentLane.Creature.HasThorns)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Thorns, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
