public class GrantVampiricToSandBlueCreaturesOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Green || lane.Creature.Data.Faction == CreatureFaction.Blue))
			{
				ApplyStatus(lane.Creature, StatusEnum.Vampiric, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
