public class GrantWeakenStrCrippleToOppTeamIfHPLow : OnAttacked
{
	public override bool OnEnable()
	{
		bool result = false;
		if (base.Owner.HPPct < base.Val1Pct)
		{
			foreach (LaneState lane in base.Owner.Owner.Opponent.Lanes)
			{
				if (lane.Creature != null)
				{
					ApplyStatus(lane.Creature, StatusEnum.WeakenSTR, base.Val2);
					ApplyStatus(lane.Creature, StatusEnum.Cripple, base.Val2);
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
