public class EraseBraveryForAreaAttackSpikeOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && (bool)Target.HasBravery)
		{
			CancelStatus(Target, StatusEnum.Bravery);
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.AttackSpike, base.Val1);
				}
			}
			return true;
		}
		return false;
	}
}
