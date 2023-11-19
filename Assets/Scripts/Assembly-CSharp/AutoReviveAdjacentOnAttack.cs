public class AutoReviveAdjacentOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && !adjacentLane.Creature.HasAutoRevive)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.AutoRevive, 1f);
				result = true;
			}
		}
		return result;
	}
}
