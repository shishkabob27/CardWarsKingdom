public class HealLowHealthCreaturesEachTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HPPct < base.Val1Pct)
			{
				lane.Creature.Heal(base.Val2Pct);
				result = true;
			}
		}
		return result;
	}
}
