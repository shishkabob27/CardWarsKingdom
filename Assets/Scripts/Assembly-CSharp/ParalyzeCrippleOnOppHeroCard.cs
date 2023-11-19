public class ParalyzeCrippleOnOppHeroCard : OnOppPlayedCard
{
	public override bool OnEnable()
	{
		bool result = false;
		if (CardPlayed.IsLeaderCard)
		{
			foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
			{
				if (lane.Creature != null)
				{
					ApplyStatus(lane.Creature, StatusEnum.Paralyze, base.Val1);
					ApplyStatus(lane.Creature, StatusEnum.Cripple, base.Val1);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
