public class GrantEnergyIfHighHP : OnStartTurn
{
	public override bool OnEnable()
	{
		if (base.Owner.HPPct >= base.Val2Pct)
		{
			base.Owner.Owner.AddActionPoints(base.Val1);
			return true;
		}
		return false;
	}
}
