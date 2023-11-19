public class BurstSTRAndBurstINTOnKill : OnKill
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val1);
				ApplyStatus(lane.Creature, StatusEnum.MagicSpike, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
