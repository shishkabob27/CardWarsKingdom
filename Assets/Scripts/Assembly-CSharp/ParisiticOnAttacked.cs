public class ParisiticOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.ParisiticArmor, base.Val2);
			return true;
		}
		return false;
	}
}
