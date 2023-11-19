public class AdjacentFortifyDEFOnHPThreshold : OnHPThreshold
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.DefenseBoost, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
