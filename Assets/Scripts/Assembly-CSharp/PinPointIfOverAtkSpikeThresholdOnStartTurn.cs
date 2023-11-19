public class PinPointIfOverAtkSpikeThresholdOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		StatusState statusState = base.Owner.StatusEffects.Find((StatusState m) => m is BurstSTR);
		if (statusState != null && statusState.Intensity >= base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Pinpoint, 1f);
			return true;
		}
		return false;
	}
}
