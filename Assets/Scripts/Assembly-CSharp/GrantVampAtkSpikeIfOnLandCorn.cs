public class GrantVampAtkSpikeIfOnLandCorn : OnStartTurn
{
	public override bool OnEnable()
	{
		StatusState statusState = base.Owner.StatusEffects.Find((StatusState s) => s is LandCorn);
		if (statusState != null)
		{
			ApplyStatus(base.Owner, StatusEnum.Vampiric, base.Val1);
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val2);
			return true;
		}
		return false;
	}
}
