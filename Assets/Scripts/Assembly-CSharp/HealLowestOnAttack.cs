public class HealLowestOnAttack : OnAttack
{
	public override bool OnEnable()
	{
		CreatureState creatureState = null;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.Damage > 0 && (creatureState == null || lane.Creature.HP < creatureState.HP))
			{
				creatureState = lane.Creature;
			}
		}
		if (creatureState != null)
		{
			creatureState.Heal(base.Val1Pct);
			return true;
		}
		return false;
	}
}
