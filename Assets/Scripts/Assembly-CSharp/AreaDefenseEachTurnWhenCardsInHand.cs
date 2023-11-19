public class AreaDefenseEachTurnWhenCardsInHand : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count >= base.Val1)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.DefenseBoost, base.Val2);
				}
			}
			return true;
		}
		return false;
	}
}
