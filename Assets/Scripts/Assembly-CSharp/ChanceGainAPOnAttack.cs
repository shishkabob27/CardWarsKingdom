public class ChanceGainAPOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			base.Owner.Owner.AddActionPoints(base.Val2);
			return true;
		}
		return false;
	}
}
