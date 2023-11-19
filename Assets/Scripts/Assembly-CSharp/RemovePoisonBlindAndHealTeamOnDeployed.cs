public class RemovePoisonBlindAndHealTeamOnDeployed : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				if (lane.Creature.IsPlagued)
				{
					CancelStatus(lane.Creature, StatusEnum.Plague);
					result = true;
				}
				if (lane.Creature.IsBlind)
				{
					CancelStatus(lane.Creature, StatusEnum.Blind);
					result = true;
				}
				if (!lane.Creature.AtFullHealth)
				{
					lane.Creature.Heal(base.Val1Pct);
					result = true;
				}
			}
		}
		return result;
	}
}
