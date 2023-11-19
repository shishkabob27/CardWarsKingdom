public class VampiricToAllOnPlagueOrPoison : OnEnemyPoisonOrPlague
{
	public override bool OnEnable()
	{
		bool result = false;
		foreach (LaneState lane in base.Owner.Owner.Lanes)
		{
			if (lane.Creature != null)
			{
				ApplyStatus(lane.Creature, StatusEnum.Vampiric, base.Val1);
				result = true;
			}
		}
		return result;
	}
}
