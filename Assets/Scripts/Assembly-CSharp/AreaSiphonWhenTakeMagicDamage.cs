public class AreaSiphonWhenTakeMagicDamage : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		if (Magic && !Physical)
		{
			foreach (LaneState thisAndAdjacentLane in Attacked.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Siphon, base.Val1);
				}
			}
			return true;
		}
		return false;
	}
}
