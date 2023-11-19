public class AllChilledOnFirstTurn : OnFirstTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.FrostArmor, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
