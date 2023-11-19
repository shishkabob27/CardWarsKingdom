public class GrantFortifyHealIfHPLowOnce : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct < base.Val1Pct && StoredValue == 0)
		{
			ApplyStatus(base.Owner, StatusEnum.DefenseBoost, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.ResistanceBoost, base.Val2);
			base.Owner.Heal(base.Val3Pct);
			StoredValue = 1;
			return true;
		}
		StoredValue = 0;
		return false;
	}
}
