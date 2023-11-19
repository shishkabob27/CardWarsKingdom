public class AreaVampiricOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.Vampiric, base.Val1);
			}
		}
		return true;
	}
}
