public class TransmorphOnAttackIfNoOtherTransmorph : OnAttack
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in Target.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature != Target && lane.Creature.HasTransmogrify)
			{
				return false;
			}
		}
		ApplyStatus(Target, StatusEnum.Transmogrify, base.Val1);
		return true;
	}
}
