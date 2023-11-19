public class GrantLandSandHealOnDie : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.LandSand, 1f);
				lane.Creature.Heal(base.Val1Pct);
				result = true;
			}
		}
		return result;
	}
}
