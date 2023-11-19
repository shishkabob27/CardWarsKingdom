public class ShieldAndCleanseAndRegenOnHPThreshold : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		float num = (float)(Attacked.HP + (int)Damage) / (float)Attacked.MaxHP;
		if (Attacked != base.Owner && Attacked.HPPct < base.Val1Pct && num >= base.Val1Pct)
		{
			ApplyStatus(Attacked, StatusEnum.Shield, base.Val2);
			ApplyStatus(Attacked, StatusEnum.Regen, base.Val3);
			ApplyStatus(Attacked, StatusEnum.RemoveStatus, base.Val4, StatusRemovalData.RemoveRandomDebuffs());
			return true;
		}
		return false;
	}
}
