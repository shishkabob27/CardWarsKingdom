public class MagicCounterOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.MagicCounterattack, base.Val2);
			return true;
		}
		return false;
	}
}
