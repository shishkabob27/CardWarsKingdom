public class AreaSandlandSiphonOnFloopDraw : OnHeroCardDraw
{
	public override bool OnEnable()
	{
		foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
		{
			if (thisAndAdjacentLane.Creature != null)
			{
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.LandSand, 1f);
				ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Siphon, base.Val1);
			}
		}
		return true;
	}
}
