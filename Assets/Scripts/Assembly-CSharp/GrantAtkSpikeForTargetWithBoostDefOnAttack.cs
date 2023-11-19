public class GrantAtkSpikeForTargetWithBoostDefOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		StatusState statusState = Target.StatusEffects.Find((StatusState s) => s is FortifyDEF);
		if (statusState != null)
		{
			CancelStatus(Target, StatusEnum.DefenseBoost);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			return true;
		}
		return false;
	}
}
