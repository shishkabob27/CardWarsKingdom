public class AreaLandSwampVampiricOnFirstAtk : OnAttack
{
	public override bool OnEnable()
	{
		StoredValue++;
		if (StoredValue == 1)
		{
			foreach (LaneState thisAndAdjacentLane in base.Owner.Lane.ThisAndAdjacentLanes)
			{
				if (thisAndAdjacentLane.Creature != null)
				{
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.LandSwamp, 1f);
					ApplyStatus(thisAndAdjacentLane.Creature, StatusEnum.Vampiric, base.Val1);
				}
			}
			return true;
		}
		return false;
	}
}
