public class GrantPinPointToCornSandTeamOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Red || lane.Creature.Data.Faction == CreatureFaction.Green))
			{
				ApplyStatus(lane.Creature, StatusEnum.Pinpoint, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
