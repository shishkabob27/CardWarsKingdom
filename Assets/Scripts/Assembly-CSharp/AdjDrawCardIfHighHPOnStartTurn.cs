public class AdjDrawCardIfHighHPOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		if (base.Owner.HPPct > base.Val1Pct)
		{
			foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
			{
				if (adjacentLane.Creature != null)
				{
					adjacentLane.Creature.DrawCard();
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
