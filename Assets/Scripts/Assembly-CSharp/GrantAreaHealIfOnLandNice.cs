public class GrantAreaHealIfOnLandNice : OnLandNice
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				thisAndAdjacentLane.Creature.Heal(base.Val1Pct);
				result = true;
			}
		}
		return result;
	}
}
