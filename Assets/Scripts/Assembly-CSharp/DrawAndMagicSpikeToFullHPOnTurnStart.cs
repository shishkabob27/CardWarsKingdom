public class DrawAndMagicSpikeToFullHPOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		int val = base.Val1;
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.AtFullHealth)
			{
				for (int i = 0; i < val; i++)
				{
					lane.Creature.DrawCard();
				}
				ApplyStatus(lane.Creature, StatusEnum.MagicSpike, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
