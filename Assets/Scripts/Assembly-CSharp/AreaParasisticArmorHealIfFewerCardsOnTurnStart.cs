public class AreaParasisticArmorHealIfFewerCardsOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.Owner.Hand.Count < base.Owner.Owner.Opponent.Hand.Count)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.ParisiticArmor, base.Val1);
					thisAndAdjacentLane.Creature.Heal(base.Val2Pct);
				}
			}
			return true;
		}
		return false;
	}
}
