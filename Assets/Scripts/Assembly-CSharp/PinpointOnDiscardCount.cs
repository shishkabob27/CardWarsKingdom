public class PinpointOnDiscardCount : OnDiscardCount
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Pinpoint, base.Val2);
		return true;
	}
}
