public class EnergySiphonOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Siphon, base.Val1);
		return true;
	}
}
