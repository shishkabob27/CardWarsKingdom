public class ResurectRandomOnKill : OnKill
{
	public override bool OnEnable()
	{
		base.Owner.Owner.ResurectRandomCreature(base.Val1Pct);
		return true;
	}
}
