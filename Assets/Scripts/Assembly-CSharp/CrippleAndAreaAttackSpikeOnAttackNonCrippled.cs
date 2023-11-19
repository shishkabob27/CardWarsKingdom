public class CrippleAndAreaAttackSpikeOnAttackNonCrippled : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && !Target.HasCripple)
		{
			ApplyStatus(Target, StatusEnum.Cripple, base.Val1);
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val2);
				}
			}
			return true;
		}
		return false;
	}
}
