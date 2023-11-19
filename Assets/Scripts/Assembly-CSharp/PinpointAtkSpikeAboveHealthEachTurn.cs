public class PinpointAtkSpikeAboveHealthEachTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct >= base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Pinpoint, 1f);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val2);
			return true;
		}
		return false;
	}
}
