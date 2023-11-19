public class AreaPinpointAndDealerOnNumCardsPlayed : OnCardThreshold
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Pinpoint, base.Val2);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Dealer, base.Val3);
			}
		}
		return true;
	}
}
