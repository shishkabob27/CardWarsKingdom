public class VampiricOnBravery : OnBravery
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val1);
		return true;
	}
}
