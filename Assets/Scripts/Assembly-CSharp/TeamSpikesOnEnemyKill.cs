public class TeamSpikesOnEnemyKill : OnEnemyDied
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.AttackSpike, base.Val1);
				ApplyStatus(lane.Creature, StatusEnum.MagicSpike, base.Val1);
			}
		}
		return true;
	}
}
