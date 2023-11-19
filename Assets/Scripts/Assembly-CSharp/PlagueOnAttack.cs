public class PlagueOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		ApplyStatus(Target, StatusEnum.Plague, base.Val1);
		return true;
	}
}
