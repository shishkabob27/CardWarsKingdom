public class GrantPinpointAdrenalineAtkSpikeOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Pinpoint, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.Adrenaline, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val2);
		return true;
	}
}
