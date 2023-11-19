public class GrantPinPointToTeamOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Pinpoint, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
