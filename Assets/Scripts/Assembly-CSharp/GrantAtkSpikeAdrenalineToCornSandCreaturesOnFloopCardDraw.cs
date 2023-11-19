public class GrantAtkSpikeAdrenalineToCornSandCreaturesOnFloopCardDraw : OnCardDraw
{
	public override bool OnEnable()
	{
		if (!CardDrawn.IsLeaderCard)
		{
			return false;
		}
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Red || lane.Creature.Data.Faction == CreatureFaction.Green))
			{
				ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val1);
				ApplyStatus(lane.Creature, StatusEnum.Adrenaline, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
