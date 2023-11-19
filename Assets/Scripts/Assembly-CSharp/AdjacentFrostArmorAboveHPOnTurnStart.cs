public class AdjacentFrostArmorAboveHPOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState adjacentLane in base.Owner.Lane.AdjacentLanes)
		{
			if (adjacentLane.Creature != null && adjacentLane.Creature.HPPct > base.Val2Pct)
			{
				ApplyStatus(adjacentLane.Creature, StatusEnum.FrostArmor, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
