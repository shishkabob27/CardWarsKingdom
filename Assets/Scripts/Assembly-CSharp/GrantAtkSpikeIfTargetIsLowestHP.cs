using UnityEngine;

public class GrantAtkSpikeIfTargetIsLowestHP : OnAttack
{
	public override bool OnEnable()
	{
		CreatureState creatureState = null;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				int num = ((lane.Creature != Target) ? lane.Creature.HP : (lane.Creature.HP + Mathf.CeilToInt(Damage)));
				if (creatureState == null || num < creatureState.HP)
				{
					creatureState = lane.Creature;
				}
			}
		}
		if (Target.HP <= 0 && Target.HP + Mathf.CeilToInt(Damage) < creatureState.HP)
		{
			creatureState = Target;
		}
		if (creatureState == Target)
		{
			ApplyStatus(base.Owner, StatusEnum.AttackSpike, base.Val1);
			return true;
		}
		return false;
	}
}
