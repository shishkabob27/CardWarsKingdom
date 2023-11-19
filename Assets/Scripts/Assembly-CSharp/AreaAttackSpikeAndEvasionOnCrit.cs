public class AreaAttackSpikeAndEvasionOnCrit : OnCritical
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val1);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Evasion, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
