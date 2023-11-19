public class RemoveNagStatusAdjCreatureOnCardPlayed : OnPlayedCardOnCreature
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.RemoveStatus, base.Val1, StatusRemovalData.RemoveRandomDebuffs());
				result = true;
			}
		}
		return result;
	}
}
