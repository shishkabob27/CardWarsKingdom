public class GrantShieldHeroicOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Shield, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val2);
		return true;
	}
}
