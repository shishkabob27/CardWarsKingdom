public class APLeakOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.ManaLeak, base.Val1);
		return true;
	}
}
