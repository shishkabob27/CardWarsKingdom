public class AutoReviveOnTurnCount : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		StoredValue++;
		if (StoredValue == base.Val1)
		{
			CreatureState creatureState = null;
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null && !lane.Creature.HasAutoRevive && (creatureState == null || lane.Creature.HP < creatureState.HP))
				{
					creatureState = lane.Creature;
				}
			}
			if (creatureState != null)
			{
				ApplyStatus(creatureState, StatusEnum.AutoRevive, 0f);
				result = true;
			}
			StoredValue = 0;
		}
		return result;
	}
}
