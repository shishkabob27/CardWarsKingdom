public class CrippleOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			ApplyStatus(Target, StatusEnum.Cripple, base.Val2);
			return true;
		}
		return false;
	}
}
