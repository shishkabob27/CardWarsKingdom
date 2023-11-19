public class PoisonAndAreaVampiricOnAttackNonPoisoned : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && !Target.IsPoisoned)
		{
			ApplyStatus(Target, StatusEnum.Poison, base.Val1);
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Vampiric, base.Val2);
				}
			}
			return true;
		}
		return false;
	}
}
