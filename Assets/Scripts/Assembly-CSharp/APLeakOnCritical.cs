public class APLeakOnCritical : OnCritical
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.ManaLeak, base.Val1);
		return true;
	}
}
