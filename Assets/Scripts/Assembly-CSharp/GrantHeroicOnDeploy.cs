public class GrantHeroicOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val1);
		return true;
	}
}
