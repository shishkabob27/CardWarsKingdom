public class GrantEnergyOnCritical : OnCritical
{
	public override bool OnEnable()
	{
		base.Owner.Owner.AddActionPoints(base.Val1);
		return true;
	}
}
