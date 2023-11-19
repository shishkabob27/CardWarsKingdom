public class GrantBoostDefnseToCornSwampCreaturesOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Red || lane.Creature.Data.Faction == CreatureFaction.Dark))
			{
				ApplyStatus(lane.Creature, StatusEnum.DefenseBoost, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
