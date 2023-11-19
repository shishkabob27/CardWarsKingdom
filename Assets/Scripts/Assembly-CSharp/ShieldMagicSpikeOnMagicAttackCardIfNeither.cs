public class ShieldMagicSpikeOnMagicAttackCardIfNeither : OnPlayedCardOnFriendlyCreature
{
	public override bool OnEnable()
	{
		if (CardPlayed.AttackBase == AttackBase.INT)
		{
			if (PlayedOnCreature.HasShield)
			{
				return false;
			}
			if (PlayedOnCreature.HasMagicSpike)
			{
				return false;
			}
			ApplyStatus(PlayedOnCreature, StatusEnum.Shield, base.Val1);
			ApplyStatus(PlayedOnCreature, StatusEnum.MagicSpike, base.Val2);
			return true;
		}
		return false;
	}
}
