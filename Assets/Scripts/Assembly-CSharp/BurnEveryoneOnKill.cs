public class BurnEveryoneOnKill : OnKill
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Burn, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
