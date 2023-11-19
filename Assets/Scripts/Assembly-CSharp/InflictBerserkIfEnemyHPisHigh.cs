public class InflictBerserkIfEnemyHPisHigh : OnAttack
{
	public override bool OnEnable()
	{
		float num = ((float)Target.HP + Damage) / (float)Target.MaxHP;
		if (num > base.Val1Pct)
		{
			ApplyStatus(Target, StatusEnum.Berserk, 1f);
			return true;
		}
		return false;
	}
}
