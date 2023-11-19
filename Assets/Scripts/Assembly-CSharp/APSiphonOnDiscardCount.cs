public class APSiphonOnDiscardCount : OnDiscardCount
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Siphon, base.Val2);
		return true;
	}
}
