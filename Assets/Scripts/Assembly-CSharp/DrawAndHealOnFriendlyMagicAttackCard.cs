public class DrawAndHealOnFriendlyMagicAttackCard : OnPlayedCardOnFriendlyCreature
{
	public override bool OnEnable()
	{
		if (CardPlayed.AttackBase == AttackBase.INT)
		{
			PlayedOnCreature.DrawCard();
			PlayedOnCreature.Heal(base.Val1Pct);
			return true;
		}
		return false;
	}
}
