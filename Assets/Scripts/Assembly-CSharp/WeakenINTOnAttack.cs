public class WeakenINTOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.WeakenMagic, base.Val2);
		return true;
	}
}
