public class DefianceOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.IgnoreDefense, base.Val2);
			return true;
		}
		return false;
	}
}
