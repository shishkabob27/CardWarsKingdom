public class RemoveNegStatusIfLowHP : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct < base.Val1Pct)
		{
			ApplyStatus(base.Owner, StatusEnum.RemoveStatus, 1f, StatusRemovalData.RemoveRandomDebuffs());
			return true;
		}
		return false;
	}
}
