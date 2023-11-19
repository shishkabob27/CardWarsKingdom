public class MagicSpikeShieldOnFriendlyMagicAttackCard : OnPlayedCardOnFriendlyCreature
{
	public override bool OnEnable()
	{
		if (CardPlayed.AttackBase == AttackBase.INT)
		{
			ApplyStatus(PlayedOnCreature, StatusEnum.Shield, base.Val1);
			ApplyStatus(PlayedOnCreature, StatusEnum.MagicSpike, base.Val2);
			return true;
		}
		return false;
	}
}
