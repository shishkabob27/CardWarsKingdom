public class GrantThornsIfLowHPOnAttack : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct < base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Thorns, base.Val2);
			return true;
		}
		return false;
	}
}
