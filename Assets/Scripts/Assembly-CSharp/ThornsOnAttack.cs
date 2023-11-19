public class ThornsOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(base.Owner, StatusEnum.Thorns, base.Val2);
			return true;
		}
		return false;
	}
}
