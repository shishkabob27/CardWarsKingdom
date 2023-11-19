public class AreaVampiricIfMoreCardsOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > base.Owner.Owner.Opponent.Hand.Count)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Vampiric, base.Val1);
				}
			}
			return true;
		}
		return false;
	}
}
