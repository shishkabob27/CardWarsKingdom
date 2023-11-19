public class AdjacentAdrenalineOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.Adrenaline, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
