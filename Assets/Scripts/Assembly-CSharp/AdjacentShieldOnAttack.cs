public class AdjacentShieldOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		if (base.Val1Chance)
		{
			foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
			{
				if (adjacentLane.Creature != null)
				{
					ApplyStatus(adjacentLane.Creature, StatusEnum.Shield, base.Val2);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
