public class AtkSpikeToSandAndSwampOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Form.Faction == CreatureFaction.Green || lane.Creature.Data.Form.Faction == CreatureFaction.Dark))
			{
				ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val1);
			}
		}
		return result;
	}
}
