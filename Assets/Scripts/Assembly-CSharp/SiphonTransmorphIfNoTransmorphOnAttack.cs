public class SiphonTransmorphIfNoTransmorphOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HasTransmogrify)
			{
				return false;
			}
		}
		if (Target != null)
		{
			if (Target.HasTransmogrify)
			{
				return false;
			}
			ApplyStatus(Target, StatusEnum.Transmorph, base.Val1);
		}
		ApplyStatus(base.Owner, StatusEnum.Siphon, base.Val2);
		return true;
	}
}
