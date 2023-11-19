public class BurstSTRAndBurstINTOnPinpoint : OnPinpoint
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.MagicSpike, base.Val2);
		return true;
	}
}
