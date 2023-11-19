public class GrantAreaBoostDefOnLandPlains : OnLandPlains
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.DefenseBoost, base.Val1);
			}
		}
		return true;
	}
}
