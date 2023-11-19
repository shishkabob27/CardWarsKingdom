public class ExtraAPOnFirstTurn : OnFirstTurn
{
	public override bool OnEnable()
	{
		int num = 0;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null && lane.Creature.Data.Faction == (CreatureFaction)base.Val2)
			{
				num++;
			}
		}
		base.Owner.Owner.AddActionPoints(base.Val1 * num);
		return num > 0;
	}
}
