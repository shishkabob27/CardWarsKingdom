public class StealAtkSpikeAndHealOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		StatusState statusState = Target.StatusEffects.Find((StatusState m) => m is BurstSTR);
		if (statusState != null)
		{
			Target.CancelStatusEffect(statusState.Data);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			base.Owner.Heal(base.Val2Pct);
			return true;
		}
		return false;
	}
}
