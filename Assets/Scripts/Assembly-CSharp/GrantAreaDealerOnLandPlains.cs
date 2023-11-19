public class GrantAreaDealerOnLandPlains : OnLandPlains
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Dealer, base.Val1);
			}
		}
		return true;
	}
}
