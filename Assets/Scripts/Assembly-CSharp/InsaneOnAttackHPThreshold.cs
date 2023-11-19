public class InsaneOnAttackHPThreshold : OnAttackHPThreshold
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Insane, base.Val2);
		return true;
	}
}
