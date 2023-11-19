public class ConfuseOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(Attacker, StatusEnum.Confuse, base.Val2);
			return true;
		}
		return false;
	}
}
