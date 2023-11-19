public class AdjCreaturesDrawCardsWhenDeployed : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				for (int i = 0; i < base.Val1; i++)
				{
					adjacentLane.Creature.DrawCard();
				}
				result = true;
			}
		}
		return result;
	}
}
