public class AdrenalineThornsOnLandSwamp : OnLandSwamp
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Adrenaline, base.Val1);
		ApplyStatus(base.Owner, StatusEnum.Thorns, base.Val1);
		return true;
	}
}
