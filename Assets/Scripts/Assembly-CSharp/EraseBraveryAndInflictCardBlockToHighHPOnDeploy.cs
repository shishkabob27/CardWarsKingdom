public class EraseBraveryAndInflictCardBlockToHighHPOnDeploy : OnDeploy
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
		{
			if (lane.Creature != null)
			{
				if ((bool)lane.Creature.HasBravery)
				{
					CancelStatus(lane.Creature, StatusEnum.Bravery);
					result = true;
				}
				if (lane.Creature.HPPct > base.Val2Pct)
				{
					ApplyStatus(lane.Creature, StatusEnum.CardBlock, base.Val1);
					result = true;
				}
			}
		}
		return result;
	}
}
