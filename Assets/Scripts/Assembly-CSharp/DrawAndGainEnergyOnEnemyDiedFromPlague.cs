public class DrawAndGainEnergyOnEnemyDiedFromPlague : OnEnemyDied
{
	public override bool OnEnable()
	{
		if (CausedByStatus != null && CausedByStatus.ID == StatusEnum.Plague.Name())
		{
			foreach (LaneState lane in base.Owner.Owner.Lanes)
			{
				if (lane.Creature != null)
				{
					lane.Creature.DrawCard();
				}
			}
			base.Owner.Owner.AddActionPoints(base.Val1);
			return true;
		}
		return false;
	}
}
