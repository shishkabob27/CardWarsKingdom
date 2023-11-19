public class GainAPOnMagicDamage : OnMagicDamage
{
	public override bool OnEnable()
	{
		base.Owner.Owner.AddActionPoints(base.Val1);
		return true;
	}
}
