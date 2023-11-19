public class HealAndImmunityToLowestHPOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		CreatureState creatureState = null;
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (creatureState == null || lane.Creature.HPPct < creatureState.HPPct))
			{
				creatureState = lane.Creature;
			}
		}
		if (creatureState != null)
		{
			if (!creatureState.AtFullHealth)
			{
				creatureState.Heal(base.Val1Pct);
				result = true;
			}
			if (!creatureState.HasImmunity)
			{
				ApplyStatus(creatureState, StatusEnum.Immunity, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
