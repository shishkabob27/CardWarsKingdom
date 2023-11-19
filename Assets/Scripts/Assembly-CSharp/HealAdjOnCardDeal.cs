public class HealAdjOnCardDeal : OnDealCard
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				adjacentLane.Creature.Heal(base.Val1);
				result = true;
			}
		}
		return result;
	}
}
