public class CleansForShieldOnStartTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HasShield)
			{
				ApplyStatus(lane.Creature, StatusEnum.RemoveStatus, base.Val1, StatusRemovalData.RemoveRandomDebuffs());
				result = true;
			}
		}
		return result;
	}
}
