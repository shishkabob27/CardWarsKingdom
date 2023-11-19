public class BraveryAndHealForHighestHPOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		CreatureState creatureState = null;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (creatureState == null || lane.Creature.HP > creatureState.HP))
			{
				creatureState = lane.Creature;
			}
		}
		if (creatureState != null)
		{
			ApplyStatus(creatureState, StatusEnum.Bravery, base.Val1);
			if (!creatureState.AtFullHealth)
			{
				creatureState.Heal(base.Val2Pct);
			}
			return true;
		}
		return false;
	}
}
