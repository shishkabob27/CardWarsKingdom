public class AdjacentHealOnEnemyCardDraw : OnOpponentCardDraw
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && !adjacentLane.Creature.AtFullHealth)
			{
				adjacentLane.Creature.Heal(base.Val1Pct);
				result = true;
			}
		}
		return result;
	}
}
