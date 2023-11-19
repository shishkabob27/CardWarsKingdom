public class HealOnCritical : OnCritical
{
	public override bool OnEnable()
	{
		if (!base.Owner.AtFullHealth)
		{
			base.Owner.Heal(base.Val1Pct);
			return true;
		}
		return false;
	}
}
