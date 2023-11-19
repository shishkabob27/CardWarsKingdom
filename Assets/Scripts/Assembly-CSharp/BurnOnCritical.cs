public class BurnOnCritical : OnCritical
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Burn, base.Val1);
		return true;
	}
}
