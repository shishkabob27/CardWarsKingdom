public class AdrenalineOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.Adrenaline, base.Val2);
		return true;
	}
}
