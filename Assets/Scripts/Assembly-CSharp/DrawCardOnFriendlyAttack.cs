public class DrawCardOnFriendlyAttack : OnFriendlyAttack
{
	public override bool OnEnable()
	{
		if (base.Val1Chance)
		{
			Attacker.DrawCard();
			return true;
		}
		return false;
	}
}
