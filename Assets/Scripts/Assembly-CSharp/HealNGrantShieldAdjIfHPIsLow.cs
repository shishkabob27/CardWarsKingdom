public class HealNGrantShieldAdjIfHPIsLow : OnFriendlyAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		if (Attacked.HPPct < base.Val1Pct)
		{
			Attacked.Heal(base.Val2Pct);
			ApplyStatus(Attacked, StatusEnum.Shield, base.Val3);
			result = true;
		}
		return result;
	}
}
