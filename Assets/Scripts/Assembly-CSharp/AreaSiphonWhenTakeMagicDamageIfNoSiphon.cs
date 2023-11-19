public class AreaSiphonWhenTakeMagicDamageIfNoSiphon : OnFriendlyDamaged
{
	public override bool OnEnable()
	{
		if (Attacked.HasSiphon)
		{
			return false;
		}
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
