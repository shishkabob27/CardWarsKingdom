public class PlagueOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		ApplyStatus(Attacker, StatusEnum.Plague, base.Val1);
		return true;
	}
}
