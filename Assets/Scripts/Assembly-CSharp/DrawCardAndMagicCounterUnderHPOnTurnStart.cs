public class DrawCardAndMagicCounterUnderHPOnTurnStart : OnStartTurn
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.HPPct < base.Val1Pct)
			{
				lane.Creature.DrawCard();
				ApplyStatus(lane.Creature, StatusEnum.MagicCounterattack, base.Val2);
				result = true;
			}
		}
		return result;
	}
}
