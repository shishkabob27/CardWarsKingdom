public class AreaDealerOnLandSwamp : OnLandSwamp
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Dealer, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
