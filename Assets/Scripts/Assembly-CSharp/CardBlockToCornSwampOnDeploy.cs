public class CardBlockToCornSwampOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null && (lane.Creature.Data.Faction == CreatureFaction.Red || lane.Creature.Data.Faction == CreatureFaction.Dark))
			{
				ApplyStatus(lane.Creature, StatusEnum.CardBlock, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
