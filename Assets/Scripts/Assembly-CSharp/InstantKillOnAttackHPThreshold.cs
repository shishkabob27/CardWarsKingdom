public class InstantKillOnAttackHPThreshold : OnAttack
{
	public override bool OnEnable()
	{
		float num = ((float)Target.HP + Damage) / (float)Target.MaxHP;
		if (num <= base.Val1Pct)
		{
			Target.DealDamage(Target.HP, AttackBase.None, false, null, null);
			return true;
		}
		return false;
	}
}
