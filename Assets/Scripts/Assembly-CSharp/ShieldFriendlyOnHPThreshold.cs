public class ShieldFriendlyOnHPThreshold : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		float num = (float)(Attacked.HP + (int)Damage) / (float)Attacked.MaxHP;
		if (Attacked.HPPct < base.Val1Pct && num >= base.Val1Pct)
		{
			ApplyStatus(Attacked, StatusEnum.Shield, base.Val2);
			return true;
		}
		return false;
	}
}
