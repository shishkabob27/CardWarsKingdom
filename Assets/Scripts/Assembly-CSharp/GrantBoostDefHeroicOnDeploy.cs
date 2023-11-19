public class GrantBoostDefHeroicOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val1);
		return true;
	}
}
