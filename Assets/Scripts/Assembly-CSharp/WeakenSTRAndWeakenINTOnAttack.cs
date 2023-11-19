public class WeakenSTRAndWeakenINTOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val2);
		ApplyStatus(Target, StatusEnum.WeakenMagic, base.Val2);
		return true;
	}
}
