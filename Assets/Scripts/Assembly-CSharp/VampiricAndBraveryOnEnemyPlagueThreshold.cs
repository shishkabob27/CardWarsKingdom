public class VampiricAndBraveryOnEnemyPlagueThreshold : OnEnemyPlague
{
	public override bool OnEnable()
	{
		StatusState statusState = CreatureAffected.StatusEffects.Find((StatusState s) => s is Plague);
		if (statusState != null && statusState.Intensity >= base.Val1Pct && statusState.Intensity - AmountChange < base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val2);
			ApplyStatus(base.Owner, StatusEnum.Bravery, base.Val3);
			return true;
		}
		return false;
	}
}
