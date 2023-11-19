public class AdjacentPinpointHealOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.Pinpoint, base.Val1);
				adjacentLane.Creature.Heal(base.Val2Pct);
			}
		}
		return true;
	}
}
