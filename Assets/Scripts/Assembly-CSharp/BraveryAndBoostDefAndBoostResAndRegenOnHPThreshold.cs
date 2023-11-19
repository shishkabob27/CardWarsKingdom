public class BraveryAndBoostDefAndBoostResAndRegenOnHPThreshold : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		float num = (float)(Attacked.HP + (int)Damage) / (float)Attacked.MaxHP;
		if (Attacked != base.Owner && Attacked.HPPct < base.Val1Pct && num >= base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.ResistanceBoost, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.Regen, base.Val2);
			return true;
		}
		return false;
	}
}
