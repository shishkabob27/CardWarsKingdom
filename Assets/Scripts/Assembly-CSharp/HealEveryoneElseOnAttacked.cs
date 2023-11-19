public class HealEveryoneElseOnAttacked : OnAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature != base.Owner && !lane.Creature.AtFullHealth)
			{
				lane.Creature.Heal(base.Val1Pct);
				result = true;
			}
		}
		return result;
	}
}
