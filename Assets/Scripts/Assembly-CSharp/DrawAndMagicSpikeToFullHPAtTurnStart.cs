public class DrawAndMagicSpikeToFullHPAtTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && adjacentLane.Creature.AtFullHealth)
			{
				for (int i = 0; i < base.Val1; i++)
				{
					adjacentLane.Creature.DrawCard();
				}
				ApplyStatus(adjacentLane.Creature, StatusEnum.MagicSpike, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
