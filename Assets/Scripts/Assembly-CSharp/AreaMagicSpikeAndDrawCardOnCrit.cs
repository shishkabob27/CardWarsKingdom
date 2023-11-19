public class AreaMagicSpikeAndDrawCardOnCrit : OnCritical
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.MagicSpike, base.Val1);
				if (thisAndAdjacentLane.Creature != base.Owner)
				{
					thisAndAdjacentLane.Creature.DrawCard();
				}
				result = true;
			}
		}
		return result;
	}
}
