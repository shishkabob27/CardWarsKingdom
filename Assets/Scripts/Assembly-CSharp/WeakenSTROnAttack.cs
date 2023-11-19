public class WeakenSTROnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val2);
		return true;
	}
}
