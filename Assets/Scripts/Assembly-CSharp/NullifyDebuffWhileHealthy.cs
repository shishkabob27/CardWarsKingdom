public class NullifyDebuffWhileHealthy : OnDebuff
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct >= base.Val1Pct && base.Owner.HasDebuff)
		{
			base.Owner.RemoveStatusEffects(StatusType.Debuff);
			return true;
		}
		return false;
	}
}
