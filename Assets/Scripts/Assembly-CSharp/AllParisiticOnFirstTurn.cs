public class AllParisiticOnFirstTurn : OnFirstTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.ParisiticArmor, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
