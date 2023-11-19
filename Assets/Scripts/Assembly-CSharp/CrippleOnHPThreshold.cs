public class CrippleOnHPThreshold : OnAttackHPThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val2);
		return true;
	}
}
