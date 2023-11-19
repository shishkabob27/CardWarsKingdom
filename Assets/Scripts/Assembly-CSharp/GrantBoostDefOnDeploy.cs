public class GrantBoostDefOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val1);
		return true;
	}
}
