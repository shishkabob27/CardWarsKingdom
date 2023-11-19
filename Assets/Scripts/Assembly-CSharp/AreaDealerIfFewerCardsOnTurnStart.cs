public class AreaDealerIfFewerCardsOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		if (base.Owner.Owner.Hand.Count < base.Owner.Owner.Opponent.Hand.Count)
		{
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
		return result;
	}
}
