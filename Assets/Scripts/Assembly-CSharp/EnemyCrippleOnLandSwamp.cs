public class EnemyCrippleOnLandSwamp : OnLandSwamp
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && !lane.Creature.HasCripple)
			{
				ApplyStatus(lane.Creature, StatusEnum.Cripple, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
