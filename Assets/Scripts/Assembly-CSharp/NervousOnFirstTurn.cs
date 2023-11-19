public class NervousOnFirstTurn : OnFirstTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Nervous, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
