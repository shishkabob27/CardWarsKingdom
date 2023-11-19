public class GrantShieldOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Shield, base.Val1);
		return true;
	}
}
