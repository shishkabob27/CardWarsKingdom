public class GrantCounterAtkOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Counterattack, base.Val1);
		return true;
	}
}
