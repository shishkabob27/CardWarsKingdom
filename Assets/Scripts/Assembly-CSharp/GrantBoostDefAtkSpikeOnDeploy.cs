public class GrantBoostDefAtkSpikeOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val2);
		return true;
	}
}
