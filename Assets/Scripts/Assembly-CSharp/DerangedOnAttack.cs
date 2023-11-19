public class DerangedOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(base.Owner, StatusEnum.VoidArmor, base.Val2);
		return true;
	}
}
