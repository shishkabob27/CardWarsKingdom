public class GrantAtkSpikePPIfOnLandNice : OnLandNice
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.Pinpoint, base.Val2);
		return true;
	}
}
