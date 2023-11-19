public class ReviveToLowHPOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HPPct < base.Val1Pct && !lane.Creature.HasAutoRevive)
			{
				ApplyStatus(lane.Creature, StatusEnum.AutoRevive, 1f);
				result = true;
			}
		}
		return result;
	}
}
