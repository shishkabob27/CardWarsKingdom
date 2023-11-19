public class AdjacentHealAndRemovePlagueEachTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null)
			{
				StatusState statusState = adjacentLane.Creature.StatusEffects.Find((StatusState s) => s is Plague);
				if (statusState != null)
				{
					CancelStatus(adjacentLane.Creature, StatusEnum.Plague);
					result = true;
				}
				if (!adjacentLane.Creature.AtFullHealth)
				{
					adjacentLane.Creature.Heal(base.Val1Pct);
					result = true;
				}
			}
		}
		return result;
	}
}
