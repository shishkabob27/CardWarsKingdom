public class CrippleIfHighHPOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		float num = ((float)Target.HP + Damage) / (float)Target.MaxHP;
		if (num > base.Val1Pct)
		{
			ApplyStatus(Target, StatusEnum.Cripple, base.Val2);
			return true;
		}
		return false;
	}
}
