public class GrantPinPointIfHPIsHigh : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct > base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Pinpoint, 1f);
			return true;
		}
		return false;
	}
}
