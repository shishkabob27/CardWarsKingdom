public class APSiphonOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.Siphon, base.Val2);
			return true;
		}
		return false;
	}
}
