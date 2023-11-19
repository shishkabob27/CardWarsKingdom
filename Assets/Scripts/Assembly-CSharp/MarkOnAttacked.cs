public class MarkOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		ApplyStatus(Attacker, StatusEnum.Marked, base.Val1);
		return true;
	}
}
