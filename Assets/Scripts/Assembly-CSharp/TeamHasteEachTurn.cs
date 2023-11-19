public class TeamHasteEachTurn : OnStartTurn
{
	public override bool OnEnable()
	{
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Adrenaline, base.Val1);
			}
		}
		return true;
	}
}
