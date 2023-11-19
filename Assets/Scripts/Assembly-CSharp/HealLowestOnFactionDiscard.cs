public class HealLowestOnFactionDiscard : OnDiscard
{
	public override bool OnEnable()
	{
		if (CardDiscared.Faction == (CreatureFaction)base.Val2)
		{
			CreatureState creatureState = null;
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null && !lane.Creature.AtFullHealth && (creatureState == null || lane.Creature.HP < creatureState.HP))
				{
					creatureState = lane.Creature;
				}
			}
			creatureState.Heal(base.Val1Pct);
			return true;
		}
		return false;
	}
}
