public class DepleteAPOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			base.Owner.Owner.Opponent.LoseActionPoints(base.Val2);
			return true;
		}
		return false;
	}
}
