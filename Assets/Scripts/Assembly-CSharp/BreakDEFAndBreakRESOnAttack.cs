public class BreakDEFAndBreakRESOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(Target, StatusEnum.BreakDefense, base.Val2);
			ApplyStatus(Target, StatusEnum.BreakResistance, base.Val2);
			return true;
		}
		return false;
	}
}
