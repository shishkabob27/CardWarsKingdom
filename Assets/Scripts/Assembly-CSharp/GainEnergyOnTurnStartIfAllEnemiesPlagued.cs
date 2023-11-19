public class GainEnergyOnTurnStartIfAllEnemiesPlagued : OnStartTurn
{
	public override bool OnEnable()
	{
		bool flag = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				if (!lane.Creature.IsPlagued && !lane.Creature.IsPoisoned)
				{
					return false;
				}
				flag = true;
			}
		}
		if (flag)
		{
			base.Owner.Owner.AddActionPoints(base.Val1);
			return true;
		}
		return false;
	}
}
