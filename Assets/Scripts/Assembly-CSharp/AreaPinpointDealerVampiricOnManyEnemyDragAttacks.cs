public class AreaPinpointDealerVampiricOnManyEnemyDragAttacks : OnEnemyAttack
{
	public override bool OnEnable()
	{
		if (Attacker.DragAttacksThisTurn == 2 && IsDragAttack)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Pinpoint, 1f);
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Dealer, 1f);
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Vampiric, base.Val1);
				}
			}
			return true;
		}
		return false;
	}
}
