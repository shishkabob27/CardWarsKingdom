public class BurstSTROnStealth : OnStealth
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		return true;
	}
}
