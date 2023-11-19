public class AreaAdrenalineAttackSpikeIfMoreCardsOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count > base.Owner.Owner.Opponent.Hand.Count)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Adrenaline, base.Val1);
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val2);
				}
			}
			return true;
		}
		return false;
	}
}
