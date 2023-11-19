public class EnergySiphonToEnemyNiceCreaturesOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && lane.Creature.Data.Faction == CreatureFaction.Light)
			{
				ApplyStatus(lane.Creature, StatusEnum.Siphon, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
