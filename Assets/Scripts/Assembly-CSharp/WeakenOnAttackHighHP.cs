public class WeakenOnAttackHighHP : OnAttack
{
	public override bool OnEnable()
	{
		float num = ((float)Target.HP + Damage) / (float)Target.MaxHP;
		if (Target != null && num > base.Val1Pct)
		{
			ApplyStatus(Target, StatusEnum.WeakenStrength, base.Val2);
			ApplyStatus(Target, StatusEnum.WeakenMagic, base.Val2);
			return true;
		}
		return false;
	}
}
