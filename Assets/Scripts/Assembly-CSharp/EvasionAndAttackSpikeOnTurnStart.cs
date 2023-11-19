public class EvasionAndAttackSpikeOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Evasion, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val2);
			}
		}
		return true;
	}
}
