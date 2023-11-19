public class APLeakOnAttackParalyzed : OnAttackParalyzed
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.ManaLeak, base.Val1);
		return true;
	}
}
