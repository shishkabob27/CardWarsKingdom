public class NervousOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		ApplyStatus(Attacker, StatusEnum.Nervous, base.Val1);
		return true;
	}
}
