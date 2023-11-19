public class RemoveBuffOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct >= base.Val1Pct && Attacker.HasBuff)
		{
			Attacker.RemoveStatusEffect(StatusType.Buff);
			return true;
		}
		return false;
	}
}
