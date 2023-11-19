public class ConfuseAndMindlessOnEnemyAttackLowHP : OnFriendlyAttacked
{
	public override bool OnEnable()
	{
		float num = (float)(Attacked.HP + (int)Damage) / (float)Attacked.MaxHP;
		if (Attacker != null && num < base.Val1Pct)
		{
			ApplyStatus(Attacker, StatusEnum.Confuse, base.Val2);
			ApplyStatus(Attacker, StatusEnum.Mindless, base.Val2);
			return true;
		}
		return false;
	}
}
