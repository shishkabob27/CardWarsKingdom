public class RegenAllOnAttack : OnPercentAttack
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Regen, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
