public class GrantAtkSpikeIfOnLandNice : OnStartTurn
{
	public override bool OnEnable()
	{
		StatusState statusState = base.Owner.StatusEffects.Find((StatusState s) => s is LandNice);
		if (statusState != null)
		{
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			return true;
		}
		return false;
	}
}
