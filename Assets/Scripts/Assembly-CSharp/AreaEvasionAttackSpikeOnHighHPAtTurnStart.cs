public class AreaEvasionAttackSpikeOnHighHPAtTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct > base.Val1Pct)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Evasion, base.Val2);
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val3);
				}
			}
			return true;
		}
		return false;
	}
}
