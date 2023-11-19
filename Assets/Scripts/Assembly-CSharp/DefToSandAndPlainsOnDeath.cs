public class DefToSandAndPlainsOnDeath : OnDied
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature != base.Owner && (lane.Creature.Data.Form.Faction == CreatureFaction.Green || lane.Creature.Data.Form.Faction == CreatureFaction.Blue))
			{
				ApplyStatus(lane.Creature, StatusEnum.DefenseBoost, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
