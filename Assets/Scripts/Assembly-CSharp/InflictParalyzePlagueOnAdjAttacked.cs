public class InflictParalyzePlagueOnAdjAttacked : OnFriendlyAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && Attacked == adjacentLane.Creature)
			{
				ApplyStatus(Attacker, StatusEnum.Paralyze, base.Val1);
				ApplyStatus(Attacker, StatusEnum.Plague, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
