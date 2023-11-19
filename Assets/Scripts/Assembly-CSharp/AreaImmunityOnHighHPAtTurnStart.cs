public class AreaImmunityOnHighHPAtTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct > base.Val1Pct)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Immunity, base.Val2);
				}
			}
			return true;
		}
		return false;
	}
}
