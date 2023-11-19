public class PinpointEveryoneOnKill : OnKill
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Pinpoint, 0f);
				result = true;
			}
		}
		return result;
	}
}
