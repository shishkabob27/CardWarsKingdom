public class InflictPlaugeOnNiceSandEnemyCreaturesOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Light || lane.Creature.Data.Faction == CreatureFaction.Green))
			{
				ApplyStatus(lane.Creature, StatusEnum.Plague, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
