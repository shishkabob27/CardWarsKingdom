public class GrantEnergyOnSufferCritical : OnSufferCritical
{
	public override bool OnEnable()
	{
		base.Owner.Owner.AddActionPoints(base.Val1);
		return true;
	}
}
