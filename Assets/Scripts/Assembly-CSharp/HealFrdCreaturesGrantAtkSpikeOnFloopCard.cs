public class HealFrdCreaturesGrantAtkSpikeOnFloopCard : OnPlayedCardOnAnyCreature
{
	public override bool OnEnable()
	{
		bool result = false;
		if (CardPlayed.IsLeaderCard)
		{
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null)
				{
					if (!lane.Creature.AtFullHealth)
					{
						lane.Creature.Heal(base.Val1Pct);
					}
					ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val2);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
