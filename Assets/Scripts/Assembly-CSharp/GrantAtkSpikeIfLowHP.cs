public class GrantAtkSpikeIfLowHP : OnHPThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val2);
		return true;
	}
}
