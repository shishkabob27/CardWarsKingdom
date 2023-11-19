public class GrantAdrenalineOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Adrenaline, base.Val1);
		return true;
	}
}
