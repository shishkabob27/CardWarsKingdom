public class EraseBraveryForAttackSpikeOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (Target != null && (bool)Target.HasBravery)
		{
			CancelStatus(Target, StatusEnum.Bravery);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			return true;
		}
		return false;
	}
}
